using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Graph
{
    public interface IGraph<T>
    {
        IObservable<IEnumerable<T>> RoutesBetween(T source, T target);
    }

    public class Graph<T> : IGraph<T>
    {
        private readonly Dictionary<T, List<T>> adjacencyList;
        public Graph(IEnumerable<ILink<T>> links)
        {
            adjacencyList = new Dictionary<T, List<T>>();

            foreach (var link in links)
            {
                if (!adjacencyList.ContainsKey(link.Source))
                    adjacencyList[link.Source] = new List<T>();

                adjacencyList[link.Source].Add(link.Target);
            }
        }

        public IObservable<IEnumerable<T>> RoutesBetween(T source, T target)
        {
            var routesSubject = new Subject<IEnumerable<T>>();
            var visited = new HashSet<T>();

            void DFS(T current, List<T> path)
            {
                if (visited.Contains(current))
                    return;

                visited.Add(current);
                path.Add(current);

                if (current.Equals(target))
                {
                    routesSubject.OnNext(path.ToList());
                }
                else if (adjacencyList.ContainsKey(current))
                {
                    foreach (var neighbor in adjacencyList[current])
                    {
                        DFS(neighbor, path.ToList());
                    }
                }

                visited.Remove(current);
                path.RemoveAt(path.Count - 1);
            }

            DFS(source, new List<T>());

            routesSubject.OnCompleted();

            return routesSubject;
        }
    }
}
