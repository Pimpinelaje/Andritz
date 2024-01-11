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

            void DFS(T current, List<T> path)
            {
                path.Add(current);

                if (current.Equals(target))
                {
                    routesSubject.OnNext(path.ToList().AsEnumerable());
                    return;
                }

                if (adjacencyList.ContainsKey(current))
                {
                    foreach (var neighbor in adjacencyList[current])
                    {
                        DFS(neighbor, new List<T>(path)); // Criar uma nova lista para cada chamada recursiva
                    }
                }
            }

            // Inicie a busca DFS
            DFS(source, new List<T>());

            // Remova a linha routesSubject.OnCompleted();, não é necessário

            return routesSubject.AsObservable();
        }

    }
}