namespace Corby.Framework
{
    /// <summary>
    /// 삭제되지 않는 프로세스 입니다. 가급적 게임이 시작되지마자 존재하도록 해야합니다.<br/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PPersistentProcessor<T> : PProcessor<T> 
        where T : PProcessor<T>
    {
        protected override void OnAwake()
        {
            // 이미 있으면 자신을 삭제하도록 합니다.
            if (HasInstance)
            {
                Destroy(gameObject);
                return;
            }
            
            base.OnAwake();
            DontDestroyOnLoad(gameObject);
        }
    }
}