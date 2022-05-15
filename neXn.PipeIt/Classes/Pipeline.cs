using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace neXn.PipeIt.Classes
{
    internal class Pipeline : IDisposable
    {
        public Models.PipeOptions PipeOptions { get; internal set; }
        public bool IsOpen { get; internal set; }
        public bool Recreate { get; set; }
        public event EventHandler Connected;
        public event EventHandler Flowing;
        internal NamedPipeServerStream PipeServer { get; set; }
        internal Task pipeTask;
        internal Action<NamedPipeServerStream> Flow;
        public Pipeline(Models.PipeOptions pipeOptions, Action<NamedPipeServerStream> flow, bool openValve = true, bool recreate = true)
        {
            this.PipeOptions = pipeOptions;
            this.Recreate = recreate;
            this.Flow = flow;

            this.Initialize();

            if (openValve)
            {
                this.Valve();
            }
        }
        private void Initialize()
        {
            this.PipeServer = new(this.PipeOptions.Name, PipeDirection.InOut, this.PipeOptions.NumberOfThreads);
            Debug.Print($"Pipe -{this.PipeOptions.Name}- created with id {this.PipeOptions.IdAsString} - Threads: {this.PipeOptions.NumberOfThreads}");
        }

        public void Valve(bool open = true)
        {
            if (open)
            {
                this.pipeTask = Task.Factory.StartNew(() =>
                {
                    this.IsOpen = true;
                    this.PipeServer.WaitForConnection();
                    this.Connected?.Invoke(this, EventArgs.Empty);
                    
                    using (StreamWriter w = new(this.PipeServer, Encoding.BigEndianUnicode, leaveOpen: true))
                    {
                        w.WriteLine($"Welcome to pipe \"{this.PipeOptions.Name}\" - Line: {this.PipeOptions.IdAsString}");
                    }

                    if (this.Flow != null)
                    {
                        this.Flowing?.Invoke(this, EventArgs.Empty);
                        this.Flow.Invoke(this.PipeServer);
                    }

                    if (this.Recreate)
                    {
                        this.Initialize();
                        this.Valve();
                    }
                });
            }
            else
            {
                this.PipeServer.Close();
                this.IsOpen = false;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.pipeTask?.Dispose();
            this.PipeServer?.Dispose();
        }
    }
}
