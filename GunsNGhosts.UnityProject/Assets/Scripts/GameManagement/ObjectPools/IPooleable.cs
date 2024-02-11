using UnityEngine;
using LucasSerrano.Pooling;

namespace GunsNGhosts
{
    /// <summary> Interfaces for components attatched to a GameObject that is part of a dynamic pool. </summary>
    public interface IPooleable
    {
        /// <summary> Pool that this object is part of. </summary>
        public DynamicPool<GameObject> Pool { get; set; }
    }
}
