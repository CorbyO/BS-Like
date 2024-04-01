namespace Corby.Framework
{
    /// <summary>
    /// Addressable 인스턴스를 감싸는 클래스입니다.
    /// </summary>
    public class AddressableInstanceWrapper
    {
        public static AddressableInstanceWrapper Empty => new(null);
        public IAddressableInstance Instance;
        /// <summary>
        /// 참조 카운트입니다.<br/>
        /// 0부터 시작합니다. 즉 0이면 1번 참조중이라는 뜻입니다.
        /// </summary>
        private uint _referenceCount;

        public AddressableInstanceWrapper(IAddressableInstance instance)
        {
            Instance = instance;
            _referenceCount = 0;
        }

        /// <summary>
        /// 참조 카운트를 증가 시킵니다.
        /// </summary>
        public void Counting()
        {
            _referenceCount++;
        }

        /// <summary>
        /// 참조 카운트를 감소 시킵니다.
        /// </summary>
        /// <returns>참조하는 곳이 하나라도 있으면, true</returns>
        public bool TryDiscounting()
        {
            if (_referenceCount == 0) return false;
            _referenceCount--;
            return true;
        }

        public bool IsEmpty => Instance == null;
    }
}