using System;
using System.Collections.Generic;
using Models.DataExtensibilityModel;

namespace TypeEmitAdministrator
{
    public class Program
    {
        static void Main(string[] args)
        {
            var extendedTables =
                new List<ExtendedTable>()
                {
                    new ExtendedTable(
                        "Customers",
                        "CustomerExtensions",
                        new List<ExtendedType>
                        {
                            new ExtendedType(typeof(Customer))
                        },
                        new List<ExtendedProperty>
                        {
                            new ExtendedProperty(typeof(DateTime?), "CreationDate"),
                            new ExtendedProperty(typeof(string), "Memo"),
                            new ExtendedProperty(typeof(Guid?), "LookupId"),
                            new ExtendedProperty(typeof(Lookup), "Lookup")
                        }),
                    new ExtendedTable(
                        "Orders",
                        "OrderExtensions",
                        new List<ExtendedType>
                        {
                            new ExtendedType(typeof(Order))
                        },
                        new List<ExtendedProperty>
                        {
                            new ExtendedProperty(typeof(string), "Memo"),
                        }),
                };

            Console.WriteLine("Writing the CustomModels.dll assembly and manifest...");
            EmitManager.CreateCustomModelAssembly(Properties.Settings.Default.CustomModelAssembly, extendedTables);

            Console.WriteLine("Altering the database...");
            DbExtensionManager.CreateExtensionTables(extendedTables);

            Console.WriteLine("Done... now run 'deploy.bat' and run the TypeEmitConsumer!");
            Console.ReadKey();
        }
    }
}
