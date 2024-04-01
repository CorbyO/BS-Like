using UnityEngine;

namespace Corby.Framework.UI
{
    public abstract class View : BaseBehavior
    {
        
    }
    public abstract class View<TThis, TPresenter> : View
        where TThis : View<TThis, TPresenter>
        where TPresenter : Presenter<TThis>
    {
        
    }
}