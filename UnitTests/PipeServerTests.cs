#pragma warning disable NUnit2005

using NUnit.Framework;
using neXn.PipeIt;
using System.IO;
using System.Text;
using System.IO.Pipes;
using System.Diagnostics;

namespace UnitTests
{
    public class PipeServerTests
    {
        private NamedPipeClientStream client;

        [SetUp]
        public void Setup()
        {
            this.client = new(".","testpipe", PipeDirection.InOut);
        }

        [Test]
        public void PipeServerSetup()
        {
            PipeServer p = new("testpipe", new System.Action<NamedPipeServerStream>((p) =>
            {
                try
                {
                    using (StreamWriter w = new(p, Encoding.BigEndianUnicode, leaveOpen: false))
                    {
                        w.WriteLine("Hello there!");
                    }
                }
                catch (System.Exception)
                {
                    
                }
            }));

            Assert.AreEqual(4, p.NumberOfThreads);
            Assert.AreEqual(4, p.pipes.Count);
            Assert.AreEqual("testpipe", p.Name);
            Assert.IsTrue(p.pipes.First.Value.Recreate);
            //Assert.IsTrue(p.pipes.First.Value.Flow == null);
            Assert.IsTrue(p.pipes.First.Value.IsOpen);

            this.client.Connect(2000);
            
            using (StreamReader sr = new(this.client, encoding: Encoding.BigEndianUnicode, leaveOpen: true))
            {
                string temp;
                while ((temp = sr.ReadLine()) != null)
                {
                    Debug.Print("Received from server: {0}", temp);
                }
            }

            using (StreamReader sr = new(this.client, encoding: Encoding.BigEndianUnicode, leaveOpen: true))
            {
                string temp;
                while ((temp = sr.ReadLine()) != null)
                {
                    Debug.Print("Received from server: {0}", temp);
                    Debug.Print($"Open pipes \"{this.client.NumberOfServerInstances}\"");
                }
            }

            //using (StreamWriter w = new(this.client))
            //{
            //    w.Write(0xFF);
            //}

            Assert.Pass();
        }

        [TearDown]
        public void TearDown()
        {
            this.client?.Dispose();
        }
    }
}