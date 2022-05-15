using System;
using System.Linq;

namespace neXn.PipeIt.Models
{
    internal class PipeOptions
    {
        public byte[] Id { get; private set; }
        public string IdAsString { get { return string.Join("", this.Id.Select(x => x.ToString("X2").ToLower())); } }
        public string Name { get; private set; }
        public int NumberOfThreads { get; internal set; }

        private readonly short idLength = 4;
        public PipeOptions(string name, int numberOfThreads)
        {
            this.Name = name;
            this.NumberOfThreads = numberOfThreads;
            this.Initialize();
        }
        private void Initialize()
        {
            this.CreateId();
        }
        private void CreateId()
        {
            Random rnd = new(BitConverter.ToInt32(Guid.NewGuid().ToByteArray()));
            this.Id = new byte[this.idLength];
            for (int i = 0; i < this.idLength; i++)
            {
                this.Id[i] = (byte)rnd.Next(0, 255);
            }
        }
    }
}
