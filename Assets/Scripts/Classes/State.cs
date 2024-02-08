/* 制作日
*　製作者
*　最終更新日
*/
using VContainer;

/// <summary>
/// フィールドに存在するもののステータス
/// </summary>
public class State : IDamageable
{
    /// <summary>
    /// ダメージハンドラ
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DamageEventHandler(object sender, DamageEventArgs e);
    /// <summary>
    /// ダメージを受けたときに発行する
    /// </summary>
    public event DamageEventHandler OnDamage;

    private StateData stateData;
    private Field field;

    //仮
    private bool tentativeBool = true;
    private object tentativeObject = new object();
    //

    [Inject]
    public State(Field field, StateData stateData)
    {
        this.field = field;
        this.stateData = stateData;
        this.field.AddDamage(this);
    }

    public void Damage(int damage)
    {

        //仮実装
        lock (tentativeObject)
        {
            if (!tentativeBool)
            {
                return;
            }
            tentativeBool = false;


            //HPを減らして、HPが０以下なら死亡フラグ
            //ダメージイベントを発行する
            DamageEventArgs damageEventArgs = new DamageEventArgs();
            damageEventArgs.aLive = true;
            stateData.hp -= damage;

            if (stateData.hp <= 0)
            {
                stateData.hp = 0;
                damageEventArgs.aLive = false;
            }
            damageEventArgs.stateData = stateData;
            OnDamage?.Invoke(this, damageEventArgs);
        }
    }

}