using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Griffin.Data.Meta
{
    class OracleSchemaReader : SchemaReader
    {
        // SchemaReader.ReadSchema
        public override TableCollection ReadSchema(DbConnection connection, DbProviderFactory factory)
        {
            var result=new TableCollection();
		
            _connection=connection;
            _factory=factory;

            var cmd=_factory.CreateCommand();
            cmd.Connection=connection;
            cmd.CommandText=TABLE_SQL;
            cmd.GetType().GetProperty("BindByName").SetValue(cmd, true, null);

            //pull the TableCollection in a reader
            using(cmd)
            {

                using (var rdr=cmd.ExecuteReader())
                {
                    while(rdr.Read())
                    {
                        Table tbl=new Table();
                        tbl.Name=rdr["TABLE_NAME"].ToString();
                        tbl.Schema = rdr["TABLE_SCHEMA"].ToString();
                        tbl.IsView=string.Compare(rdr["TABLE_TYPE"].ToString(), "View", true)==0;
                        tbl.CleanName=CleanUp(tbl.Name);
                        tbl.ClassName=Inflector.Instance.MakeSingular(tbl.CleanName);
                        result.Add(tbl);
                    }
                }
            }

            foreach (var tbl in result)
            {
                tbl.Columns=LoadColumns(tbl);
		            
                // Mark the primary key
                string PrimaryKey=GetPK(tbl.Name);
                var pkColumn=tbl.Columns.SingleOrDefault(x=>x.Name.ToLower().Trim()==PrimaryKey.ToLower().Trim());
                if(pkColumn!=null)
                    pkColumn.IsPrimaryKey=true;
            }
	    

            return result;
        }
	
        DbConnection _connection;
        DbProviderFactory _factory;
	

        List<Column> LoadColumns(Table tbl)
        {
	
            using (var cmd=_factory.CreateCommand())
            {
                cmd.Connection=_connection;
                cmd.CommandText=COLUMN_SQL;
                cmd.GetType().GetProperty("BindByName").SetValue(cmd, true, null);

                var p = cmd.CreateParameter();
                p.ParameterName = ":tableName";
                p.Value=tbl.Name;
                cmd.Parameters.Add(p);

                var result=new List<Column>();
                using (IDataReader rdr=cmd.ExecuteReader())
                {
                    while(rdr.Read())
                    {
                        Column col=new Column();
                        col.Name=rdr["ColumnName"].ToString();
                        col.PropertyName=CleanUp(col.Name);
                        col.PropertyType=GetPropertyType(rdr["DataType"].ToString(), (rdr["DataType"] == DBNull.Value ? null : rdr["DataType"].ToString()));
                        col.IsNullable=rdr["IsNullable"].ToString()=="YES";
                        col.IsAutoIncrement=true;
                        result.Add(col);
                    }
                }

                return result;
            }
        }

        string GetPK(string table){
		
            string sql=@"select column_name from USER_CONSTRAINTS uc
  inner join USER_CONS_COLUMNS ucc on uc.constraint_name = ucc.constraint_name
where uc.constraint_type = 'P'
and uc.table_name = upper(:tableName)
and ucc.position = 1";

            using (var cmd=_factory.CreateCommand())
            {
                cmd.Connection=_connection;
                cmd.CommandText=sql;
                cmd.GetType().GetProperty("BindByName").SetValue(cmd, true, null);

                var p = cmd.CreateParameter();
                p.ParameterName = ":tableName";
                p.Value=table;
                cmd.Parameters.Add(p);

                var result=cmd.ExecuteScalar();

                if(result!=null)
                    return result.ToString();    
            }	         
		
            return "";
        }
	
        string GetPropertyType(string sqlType, string dataScale)
        {
            string sysType="string";
            switch (sqlType.ToLower()) 
            {
                case "bigint":
                    sysType = "long";
                    break;
                case "smallint":
                    sysType= "short";
                    break;
                case "int":
                    sysType= "int";
                    break;
                case "uniqueidentifier":
                    sysType=  "Guid";
                    break;
                case "smalldatetime":
                case "datetime":
                case "date":
                    sysType=  "DateTime";
                    break;
                case "float":
                    sysType="double";
                    break;
                case "real":
                case "numeric":
                case "smallmoney":
                case "decimal":
                case "money":
                case "number":
                    sysType=  "decimal";
                    break;
                case "tinyint":
                    sysType = "byte";
                    break;
                case "bit":
                    sysType=  "bool";
                    break;
                case "image":
                case "binary":
                case "varbinary":
                case "timestamp":
                    sysType=  "byte[]";
                    break;
            }
		
            if (sqlType == "number" && dataScale == "0")
                return "long";
		
            return sysType;
        }



        const string TABLE_SQL=@"select TABLE_NAME from USER_TableCollection";

        const string COLUMN_SQL=@"select table_name TableName, 
 column_name ColumnName, 
 data_type DataType, 
 data_scale DataScale,
 nullable IsNullable
 from USER_TAB_COLS utc 
 where table_name = upper(:tableName)
 order by column_id";
	  
    }
}