using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;

namespace ElasticSearch
{
    
    public class Program
    {
        public static Uri node;
        public static ConnectionSettings settings;
        public static ElasticClient client;


        static void Main(string[] args)
        {
            node = new Uri("http://172.16.14.68:9200/");
            var settings = new ConnectionSettings(node);
            client = new ElasticClient(settings);

            Console.WriteLine("Enter the operation you Want to perform 1)Insert 2)Delete 3)Search");
            var choice = Console.ReadLine();

            bool result = Int32.TryParse(choice, out int ch);

            {
                while (true)
                {
                    switch (ch)
                    {

                        case 1:
                            Insert();
                            break;

                        case 2:

                            DeleteOperation();
                            break;

                        case 3:
                            SearchOperation();
                            break;
                    }
                }
            }
        }
        public static void Insert()
        {
            Employee employee = new Employee();
            Console.WriteLine("Enter Id");
            employee.Id = Console.ReadLine();
            Console.WriteLine("Enter First Name");
            employee.FName = Console.ReadLine();
            Console.WriteLine("Enter Middle Name");
            employee.MName = Console.ReadLine();
            Console.WriteLine("Enter Last Name");
            employee.LNAme =Console.ReadLine();
            Console.WriteLine("Enter Address");
            employee.Address = Console.ReadLine();
            AddIndex(employee);
           
        }


        public static void DeleteOperation()
        {
            Console.WriteLine("Enter the Id you want to delete");
            var result = Console.ReadLine();
            Delete(result);

        }

        public static bool Delete(string word)
        {
            bool status;

            var response = client.Delete<Employee>(word, d => d
                .Index("employee")
                .Type("mydata"));

            if (response.IsValid)
            {
                status = true;
            }
            else
            {
                status = false;
            }

            return status;

        }

        public static void AddIndex(Employee employee)
        {
            var jsonData = new Employee
            {
                Id = employee.Id,
                FName = employee.FName,
                MName = employee.MName,
                LNAme = employee.LNAme,
                Address = employee.Address,
            };
            client.Index<Employee>(jsonData, x => x.Index("employee").Type("mydata").Id(employee.Id).Refresh(Elasticsearch.Net.Refresh.True));

        }

        public static void SearchOperation()
        {
            Console.WriteLine("Enter the Id you want to search");
            var result = Console.ReadLine();
            List<Employee> listOfEmployee = Search(result);

        }

        public static List<Employee> Search(string word)
        {
            List<Employee> employees = new List<Employee>();
            var response = client.Search<Employee>(x => x.Index("employee").Type("mydata").Query(q => q.Match(t => t.Field("id").Query(word))));
            foreach (var hit in response.Hits)
            {
                var employee = new Employee();
                employee.Id = hit.Id.ToString();
                employee.FName = hit.Source.FName.ToString();
                employee.MName = hit.Source.MName.ToString();
                employee.LNAme = hit.Source.LNAme.ToString();
                employee.Address = hit.Source.Address.ToString();
                employees.Add(employee);
            }
            return employees;
        }
    }
}
