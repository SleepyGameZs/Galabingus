using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Matthew Rodriguez
// 2 / 21 / 2023
// Custom Linked Node
//  Based on the Node in a Linked List data structure.
// GalabingusLinkedList(T data)
//  - constructor to create a Custom Linked Node
// Data: info inside the Node
//  - property
// Next: next Node
//  - property

namespace Galabingus
{
    /// <summary>
    ///  Custom linked Node
    ///   Node of a Linked List data structure: Node[ data, node* ] -> Node[ data, node* ] -> Node[ data, null ]
    ///   - Holds the data of the Linked List Node
    ///   - Holds the reference to another Linked List Node
    /// </summary>
    /// <typeparam name="T">Type of data inside the Custom Linked Node</typeparam>
    internal class GalabingusLinkedNode<T>
    {
        // data: the information inside the Custom Linked List
        // next: the next node in the Custom Linked List
        // both data and next can be null; next is null at construction
        private T? data;
        private GalabingusLinkedNode<T>? next; // In C# all classes are stored by reference,
                                            // no pointers are needed,
                                            // memory verifiably safe context here / no unsafe context here
        /// <summary>
        ///  Data info inside the Node,
        ///  Set the info inside the Node
        ///  Get the info inside the node
        /// </summary>
        public T? Data
        {
            get
            {
                // the info which can be null
                return data;
            }
            set
            {
                // the info to set to can be null
                data = value;
            }
        }

        /// <summary>
        ///  Next node in the Custom Linked List
        ///  get: get the reference to the next Node
        ///  set: set the reference to the next Node
        /// </summary>
        public GalabingusLinkedNode<T>? Next
        {
            get
            {
                // the next Node can be null
                // when it is null that is the end 
                return next;
            }
            set
            {
                // the next Node is set to the Node value
                // the Node value can be null
                next = value;
            }
        }

        /// <summary>
        ///  Creates a Custom Linked Node
        ///  data: info inside, 
        ///  next: reference to the next node is null by default
        /// </summary>
        /// <param name="data">Set the info inside the node</param>
        public GalabingusLinkedNode(T data)
        {
            // Directly sets the value of data
            // Default the next Node reference to null
            this.data = data;
            this.next = null;
        }
    }
}
