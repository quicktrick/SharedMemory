namespace SharedMemory
{
    /// <summary>
    /// A <see cref="CircularBuffer"/> wrapper implementing some common collection functionality.
    /// </summary>
    public class CBuffer : CircularBuffer
    {
        #region Public/Protected properties
        /// <summary>
        /// Gets the number of elements contained in the <see cref="CBuffer"/>.
        /// </summary>
        public int Count { get { cBufferCounter.Read(out count); return count; } }
        /// <summary>
        /// Returns true if the <see cref="CBuffer"/> is full.
        /// </summary>
        public bool IsFull { get { cBufferCounter.Read(out count); return count >= nodeCountDecr; } }
        /// <summary>
        /// Returns true if the <see cref="CBuffer"/> is empty.
        /// </summary>
        public bool IsEmpty { get { cBufferCounter.Read(out count); return count == 0; } }
        #endregion // Public/Protected properties

        #region Private field members
        int count;
        int nodeCountDecr;
        BufferReadWrite cBufferCounter;
        #endregion // Private field members

        #region Constructors
        /// <summary>
        /// Inherits <see cref="CircularBuffer"/> constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nodeCount"></param>
        /// <param name="nodeBufferSize"></param>
        public CBuffer(string name, int nodeCount, int nodeBufferSize) : base(name, nodeCount, nodeBufferSize)
        {
            cBufferCounter = new BufferReadWrite(name: name + "_cntr", bufferSize: FastStructure.SizeOf<int>());
            cBufferCounter.Write(ref count);
        }
        /// <summary>
        /// Inherits <see cref="CircularBuffer"/> constructor.
        /// </summary>
        /// <param name="name"></param>
        public CBuffer(string name) : base(name)
        {
            cBufferCounter = new BufferReadWrite(name: name + "_cntr");
        }
        #endregion // Constructors

        protected override bool DoOpen()
        {
            bool result = base.DoOpen();
            nodeCountDecr = NodeCount - 1;
            return result;
        }

        protected override void DoClose()
        {
            base.DoClose();
            cBufferCounter.Close();
        }

        /// <summary>
        /// Writes the structure to the <see cref="CBuffer"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public override int Write<T>(ref T source, int timeout = 1000)
        {
            cBufferCounter.Read(out count);
            if (count < nodeCountDecr)
            {
                count++;
                cBufferCounter.Write(ref count);
                return base.Write(ref source, timeout);
            }
            return -1;
        }

        /// <summary>
        /// Reads the structure out of the <see cref="CBuffer"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="destination"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public override int Read<T>(out T destination, int timeout = 1000)
        {
            cBufferCounter.Read(out count);
            count--;
            cBufferCounter.Write(ref count);
            return base.Read(out destination, timeout);
        }

    } // class CBuffer
} // ns
