using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePresenter<TModel, TView> where TModel : BaseModel where TView : BaseView
{
    protected TModel Model { get; private set; }
    protected TView View { get; private set; }

    public virtual void Initialize(TModel model, TView view)
    {
        Model = model;
        View = view;

        Model.Initialize();
        View.Initialize();

        OnInitialize();
    }

    protected virtual void OnInitialize() { }
    public virtual void Show() => View.Show();
    public virtual void Hide() => View.Hide();
    public virtual void Dispose() => Model.Dispose();
}
