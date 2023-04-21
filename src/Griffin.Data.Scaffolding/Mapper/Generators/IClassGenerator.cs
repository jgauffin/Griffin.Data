namespace Griffin.Data.Scaffolding.Mapper.Generators;

public interface IClassGenerator
{
    void Generate(Table table, GeneratorContext context);
}
