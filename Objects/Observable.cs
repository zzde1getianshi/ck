using System;
using PropertyChanged;

namespace Pixeval.Objects
{
    [AddINotifyPropertyChangedInterface]
    public class Observable<T>
    {
        private T value;

        public T Value
        {
            get => value;
            set
            {
                if (!this.value.Equals(value))
                {
                    var old = this.value;
                    this.value = value;
                    ValueChanged?.Invoke(Value, new ObservableValueChangedEventArgs<T>(old, this.value));
                }
            }
        }

        public event EventHandler<ObservableValueChangedEventArgs<T>> ValueChanged;

        public Observable(T value)
        {
            this.value = value;
        }
    }

    public class ObservableValueChangedEventArgs<T> : EventArgs
    {
        public T OldValue { get; set; }

        public T NewValue { get; set; }

        public ObservableValueChangedEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}