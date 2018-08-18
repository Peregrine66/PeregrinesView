using System;
using System.Collections.Generic;

namespace Peregrine.Library.Collections
{
    // Helper classes for perWeakWeakDictionary
    
    // Based on Nick Guerrera: Presenting WeakDictionary[TKey, TValue]
    // https://blogs.msdn.microsoft.com/nicholg/2006/06/04/presenting-weakdictionarytkey-tvalue/

    // Adds strong typing to WeakReference.Target using generics. Also, the Create factory method is used in place of a constructor
    // to handle the case where target is null, but we want the reference to still appear to be alive.
    internal class perWeakReference<T> where T : class
    {
        private readonly WeakReference _reference;

        protected perWeakReference(T target)
        {
            _reference = new WeakReference(target, false);
        }

        public static perWeakReference<T> Create(T target)
        {
            return target == null ? perWeakNullReference<T>.Singleton : new perWeakReference<T>(target);
        }

        public T Target => _reference.Target as T;

        public virtual bool IsAlive => _reference.IsAlive;
    }

    // Provides a weak reference to a null target object, which, unlike other weak references, is always considered to be alive. 
    // This facilitates handling null dictionary values, which are perfectly legal.
    internal class perWeakNullReference<T> : perWeakReference<T> where T : class
    {
        public static readonly perWeakNullReference<T> Singleton = new perWeakNullReference<T>();

        private perWeakNullReference() : base(null) { }

        public override bool IsAlive => true;
    }

    // Provides a weak reference to an object of the given type to be used in a perWeakWeakDictionary along with the given comparer.
    internal sealed class perWeakKeyReference<T> where T : class
    {
        private readonly WeakReference _reference;

        public perWeakKeyReference(T key, IEqualityComparer<object> comparer)
        {
            _reference = new WeakReference(key, false);

            // retain the object's hash code immediately so that even if the target is GC'ed we will be able to find and remove the dead weak reference.
            HashCode = comparer.GetHashCode(key);
        }

        public int HashCode { get; }

        public T Target => _reference.Target as T;

        public bool IsAlive => _reference.IsAlive;
    }

    // Compares objects of the given type or WeakKeyReferences to them for equality based on the given comparer. Note that we can only
    // implement IEqualityComparer<T> for T = object as there is no other common base between T and WeakKeyReference<T>. We need a
    // single comparer to handle both types because we don't want to allocate a new weak reference for every lookup.
    internal sealed class perWeakKeyComparer<T> : IEqualityComparer<object>
        where T : class
    {
        private readonly IEqualityComparer<T> _comparer;

        internal perWeakKeyComparer(IEqualityComparer<T> comparer)
        {
            if (comparer == null)
                comparer = EqualityComparer<T>.Default;

            _comparer = comparer;
        }

        public int GetHashCode(object obj)
        {
            var perWeakKey = obj as perWeakKeyReference<T>;
            return perWeakKey != null ? perWeakKey.HashCode : _comparer.GetHashCode((T)obj);
        }

        // Note: There are actually 9 cases to handle here.
        //
        //  Let Wa = Alive Weak Reference
        //  Let Wd = Dead Weak Reference
        //  Let S  = Strong Reference
        //  
        //  x  | y  | Equals(x,y)
        // -------------------------------------------------
        //  Wa | Wa | comparer.Equals(x.Target, y.Target)
        //  Wa | Wd | false
        //  Wa | S  | comparer.Equals(x.Target, y)
        //  Wd | Wa | false
        //  Wd | Wd | x == y
        //  Wd | S  | false
        //  S  | Wa | comparer.Equals(x, y.Target)
        //  S  | Wd | false
        //  S  | S  | comparer.Equals(x, y)
        // -------------------------------------------------
        public new bool Equals(object x, object y)
        {
            bool xIsDead, yIsDead;
            var first = GetTarget(x, out xIsDead);
            var second = GetTarget(y, out yIsDead);

            if (xIsDead)
                return yIsDead && x == y;

            return !yIsDead && _comparer.Equals(first, second);
        }

        private static T GetTarget(object obj, out bool isDead)
        {
            var wref = obj as perWeakKeyReference<T>;
            T target;
            if (wref != null)
            {
                target = wref.Target;
                isDead = !wref.IsAlive;
            }
            else
            {
                target = (T)obj;
                isDead = false;
            }

            return target;
        }
    }
}
