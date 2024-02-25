using System;

namespace InazumaSearchLib.Core
{
    /// <summary>
    /// 進捗ゲージの状態を表すオブジェクト。主に<see cref="Progress{T}"/>クラスで使用
    /// </summary>
    public class ProgressGuageState
    {
        /// <summary>
        /// 現在値
        /// </summary>
        public virtual int Value { get; set; } = 0;

        /// <summary>
        /// 最大値
        /// </summary>
        public virtual int Maximum { get; set; } = 1000;

        /// <summary>
        /// 不確定状態かどうか
        /// </summary>
        public virtual bool Indeterminate { get; set; } = false;
    }
}
