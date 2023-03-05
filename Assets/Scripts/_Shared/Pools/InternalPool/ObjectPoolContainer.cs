
namespace BW.Pooling
{
	public class ObjectPoolContainer<T>
	{
		private T item;

		public bool Used { get; private set; }

		public void Consume()
		{
			Used = true;
		}

		public T Item
		{
			get
			{
				return this.item;
			}
			set
			{
				this.item = value;
			}
		}

		public void Release()
		{
			Used = false;
		}
	}
}
