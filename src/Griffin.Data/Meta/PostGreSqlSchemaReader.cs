using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Griffin.Data.Meta
{
    class PostGreSqlSchemaReader : SchemaReader
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

            //pull the TableCollection in a reader
            using(cmd)
            {
                using (var rdr=cmd.ExecuteReader())
                {
                    while(rdr.Read())
                    {
                        Table tbl=new Table();
                        tbl.Name=rdr["table_name"].ToString();
                        tbl.Schema=rdr["table_schema"].ToString();
                        tbl.IsView=string.Compare(rdr["table_type"].ToString(), "View", true)==0;
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

                var p = cmd.CreateParameter();
                p.ParameterName = "@tableName";
                p.Value=tbl.Name;
                cmd.Parameters.Add(p);

                var result=new List<Column>();
                using (IDataReader rdr=cmd.ExecuteReader())
                {
                    while(rdr.Read())
                    {
                        Column col=new Column();
                        col.Name=rdr["column_name"].ToString();
                        col.PropertyName=CleanUp(col.Name);
                        col.PropertyType=GetPropertyType(rdr["udt_name"].ToString());
                        col.IsNullable=rdr["is_nullable"].ToString()=="YES";
                        col.IsAutoIncrement = rdr["column_default"].ToString().StartsWith("nextval(");
                        result.Add(col);
                    }
                }

                return result;
            }
        }

        string GetPK(string table){
		
            string sql=@"SELECT kcu.column_name 
			FROM information_schema.key_column_usage kcu
			JOIN information_schema.table_constraints tc
			ON kcu.constraint_name=tc.constraint_name
			WHERE lower(tc.constraint_type)='primary key'
			AND kcu.table_name=@tablename";

            using (var cmd=_factory.CreateCommand())
            {
                cmd.Connection=_connection;
                cmd.CommandText=sql;

                var p = cmd.CreateParameter();
                p.ParameterName = "@tableName";
                p.Value=table;
                cmd.Parameters.Add(p);

                var result=cmd.ExecuteScalar();

                if(result!=null)
                    return result.ToString();    
            }	         
		
            return "";
        }
	
        string GetPropertyType(string sqlType)
        {
            switch (sqlType)
            {
                case "int8":
                case "serial8":	
                    return "long";

                case "bool":	
                    return "bool";

                case "bytea	":	
                    return "byte[]";

                case "float8":	
                    return "double";

                case "int4":	
                case "serial4":	
                    return "int";

                case "money	":	
                    return "decimal";

                case "numeric":	
                    return "decimal";

                case "float4":	
                    return "float";

                case "int2":	
                    return "short";

                case "time":
                case "timetz":
                case "timestamp":
                case "timestamptz":	
                case "date":	
                    return "DateTime";

                default:
                    return "string";
            }
        }



        const string TABLE_SQL=@"
			SELECT table_name, table_schema, table_type
			FROM information_schema.TableCollection 
			WHERE (table_type='BASE TABLE' OR table_type='VIEW')
				AND table_schema NOT IN ('pg_catalog', 'information_schema');
			";

        const string COLUMN_SQL=@"
			SELECT column_name, is_nullable, udt_name, column_default
			FROM information_schema.columns 
			WHERE table_name=@tableName;
			";
	
    }
}