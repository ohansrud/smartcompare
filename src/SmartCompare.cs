using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Reflection;

namespace SmartCompare
{
    public class CompareResult<T>
    {
        public List<T> Added { get; set; }
        public List<T> Deleted { get; set; }
        public List<T> Altered { get; set; }
        public List<T> Unaltered { get; set; }

        public CompareResult()
        {
            this.Altered = new List<T>();
            this.Unaltered = new List<T>();
            this.Deleted = new List<T>();
            this.Added = new List<T>();
        }
    }

    public static class Comparer
    {
        public static bool PublicInstancePropertiesEqual<T>(this T self, T to, params string[] ignore) where T : class
        {
            if ((self != null) && (to != null))
            {
                var type = typeof(T);
                var ignoreList = new List<string>(ignore);
                var unequalProperties =
                    from pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    where !ignoreList.Contains(pi.Name)
                    let selfValue = type.GetProperty(pi.Name).GetValue(self, null)
                    let toValue = type.GetProperty(pi.Name).GetValue(to, null)
                    where (selfValue != toValue) && ((selfValue == null) || !selfValue.Equals(toValue))
                    select selfValue;
                return !unequalProperties.Any();
            }
            return self == to;
        }


        public static CompareResult<T> ListComparer<T>(List<T> oldList, List<T> newList, string key, string[] ignore)
        {
            var res = new CompareResult<T>();
            foreach (var to in oldList)
            {
                var h = Convert.ToInt32(to.GetType().GetProperty(key).GetValue(to, null));

                var pred = key + " == " + h;

                var self = newList.Where(pred).FirstOrDefault();
                if(self == null)
                {
                    res.Deleted.Add(to);
                }
            }

            foreach (var to in newList.Except(res.Added).Except(res.Deleted))
            {
                var h = Convert.ToInt32(to.GetType().GetProperty(key).GetValue(to, null));

                var pred = key + " == " + h;

                var self = oldList.Where(pred).FirstOrDefault();
                if (self == null)
                {
                    res.Added.Add(to);
                }
                else if ((self != null) && (to != null))
                {
                    var type = typeof(T);
                    var ignoreList = new List<string>(ignore);
                    var unequalProperties =
                        from pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        where !ignoreList.Contains(pi.Name)
                        let selfValue = type.GetProperty(pi.Name).GetValue(self, null)
                        let toValue = type.GetProperty(pi.Name).GetValue(to, null)
                        where (selfValue != toValue) && ((selfValue == null) || !selfValue.Equals(toValue))
                        select selfValue;
                    if (unequalProperties.Any())
                        res.Altered.Add(to);
                    else
                        res.Unaltered.Add(to);
                }
            }

            return res;
        }
    }
}