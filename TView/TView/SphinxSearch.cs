using System.Collections.Generic;
using System.Linq;
using SphinxSearchApi;

namespace TView
{
    public class SphinxSearch
    {
        private readonly SphinxClient sc;

        public SphinxSearch(string host, int port)
        {
            sc = new SphinxClient(host, port);
        }

        public List<long> GetResult(string query, string index, int limit=1000000)
        {
            List<long> id = new List<long>();
            sc.SetLimits(0, limit);
            sc.AddQuery(query, index);
            SphinxResult[] result = sc.RunQueries();
            foreach (SphinxResult oneResult in result)
            {
                foreach (SphinxMatch match in oneResult.matches)
                    id.Add(match.docId);
            }
            return id;
        }

        public string GetIDstring(List<long> IDs)
        {
            string result = IDs.Select(i => i.ToString()).Aggregate((s1, s2) => s1 + "," + s2);
            return result;
        }
    }
}