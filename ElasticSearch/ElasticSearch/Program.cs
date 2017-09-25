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
            Employee employee = new Employee();

            employee.Id = "A101";
            employee.FName = "Amit";
            employee.MName = "NA";
            employee.LNAme = "Prakash";
            employee.Address = "Pune";
            AddIndex(employee);
            List<Employee> listOfEmployee = Search("A101");
        }

        public static void AddIndex(Employee employee)
        {
            var jsonData = new Employee
            {
                Id = employee.Id,
                FName = employee.FName,
                MName=employee.MName,
                LNAme=employee.LNAme,
                Address=employee.Address,
            };
            client.Index<Employee>(jsonData, x => x.Index("employee").Type("mydata").Id(employee.Id).Refresh(Elasticsearch.Net.Refresh.True));

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
