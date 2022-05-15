using System;
using System.Linq;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace neXn.PipeIt
{
    public class PipeServer
    {
        public string Name { get; internal set; }
        public int NumberOfThreads { get; internal set; }
        public Action<NamedPipeServerStream> Flow { get; internal set; }

        internal readonly LinkedList<Classes.Pipeline> pipes = new();

        #region Constructor
        /// <summary>
        /// Pipe constructor or, the datastream plumber
        /// </summary>
        /// <param name="name">Name your named pipe</param>
        /// <param name="threads"></param>
        public PipeServer(string name, Action<NamedPipeServerStream> flow, int threads = 4)
        {
            this.Name = name;
            this.NumberOfThreads = threads;
            this.Flow = flow;

            this.Initialize();
        }
        private void Initialize()
        {
            for (int i = 0; i < this.NumberOfThreads; i++)
            {
                Classes.Pipeline p = new(new Models.PipeOptions(this.Name, this.NumberOfThreads), this.Flow);
                if (this.pipes.Count == 0)
                {
                    this.pipes.AddFirst(p);
                }
                else
                {
                    this.pipes.AddAfter(this.pipes.Last, p);
                }
            }
        }
        #endregion

    }
}