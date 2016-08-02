using System.Collections.Generic;

namespace System
{
    public class Optional<T>
    {
        private T obj;
        public Optional()
        {
            obj = default(T);
        }
        public Optional(T obj)
        {
            this.obj = obj;
        }

        public T get()
        {
            return obj;
        }

        public T get(T def)
        {
            return isPresent() ? get() : def;
        }

        public void set(T obj)
        {
            this.obj = obj;
        }

        public bool isPresent()
        {
            return !EqualityComparer<T>.Default.Equals(obj, default(T));
        }

        public static Optional<T> of(T obj)
        {
            return new Optional<T>(obj);
        }

        public static Optional<T> absent()
        {
            return new Optional<T>();
        }

        public static Optional<T> fromNullable(T obj)
        {
            if (obj == null)
            {
                return absent();
            }

            return of(obj);
        }

    }
}
