using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Matthew Rodriguez
// 3 / 28 / 2023
// Galabingus Linked List
//  Based on the Linked List data structure
// GalabingusLinkedList( params T data )
//  - both the default and parametrized constructor for a Custom Linked List
// GalabingusLinkedList[ uint index ]
//  - used to get and set a Custom Linked Node's Data
// Count
//  Number of items in the Custom Linked List
// Add( T data )
//  - adds data to the Custom Linked List
// GetData( int index )
//  - used to get a specific item at the given index

namespace Galabingus
{
    /// <summary>
    ///  Custom Linked List
    ///  Stores data of type T in Custom Linked Nodes
    ///   Based on the Linked List data structure. Head -> First Node...Last Node <- Tail
    ///   Head: head of the Linked List
    ///   Tail: tail of the Linked List
    /// </summary>
    /// <typeparam name="T">type of data to be stored in the Custom Linked List</typeparam>
    internal class GalabingusLinkedList<T>
    {
        // Head is the head of the Linked List data structure head -> First Node
        // Tail is the tail of the Linked List data structure tail -> Last Node
        // Head always points to the first node, tail always points ot the last node
        // Count is the number of Nodes in the Custom Linked List 
        private GalabingusLinkedNode<T>? head;
        private GalabingusLinkedNode<T>? tail;
        private uint count;

        /// <summary>
        ///  Traverses through the Custom Linked List to the given index
        ///  At the given index there are Get and Set options:
        ///  Set: sets the Custom Linked Node at the given index within the Custom Linked List
        ///  Get: retrieves a Custom Linked Node at the given index within the Custom linked List
        /// </summary>
        /// <param name="index">index of the Custom Linked Node in the Custom linked List</param>
        /// <returns>Custom Linked Node at index</returns>
        /// <exception cref="IndexOutOfRangeException">index inputted was out of the range of the Custom Linked List</exception>
        /// <exception cref="Exception">Custom Linked List was cutoff by some internal add-on method</exception>
        public T? this[int index]
        {
            get
            {
                // Check to see if the index is out of bounds
                if (index >= count || count == 0 || index < 0)
                {
                    // TODO: throw out of bounds exception
                    throw new IndexOutOfRangeException($"Error: Cannot get data from invalid index {index}");
                }

                // When the list is cut-off the head will be null
                if (head == null)
                {
                    // The Linked List was cutoff
                    // TODO: throw internal error exception
                    throw new Exception($"Error: Linked list was cutoff");
                }
                else
                {
                    // By logic current will never be null here
#pragma warning disable CS8602
                    // Go through next Nodes until the node is the node at the index we are searching for
                    GalabingusLinkedNode<T>? current = head;
                    for (uint i = 0; i < index; i++)
                    {
                        current = current.Next;
                    }
                    // Return the node's data
                    return current.Data;
#pragma warning restore CS8602
                }
            }
            set
            {
                // Check to see if the index is out of bounds
                if (index >= count || count == 0 || index < 0)
                {
                    // TODO: throw out of bounds exception
                    throw new IndexOutOfRangeException($"Error: Cannot get data from invalid index {index}");
                }
                else
                {
                    // By logic current will never be null here
#pragma warning disable CS8602
                    // Go through next Nodes until the node is the node at the index we are searching for
                    GalabingusLinkedNode<T>? current = head;
                    for (uint i = 0; i < index; i++)
                    {
                        current = current.Next;
                    }
                    // Set the node's data to the value
                    current.Data = value;
#pragma warning restore CS8602
                }
            }
        }

        /// <summary>
        ///  The count of nodes inside the Custom Linked List
        ///  Get: Retrieves the number of nodes inside the Custom Linked List
        /// </summary>
        public uint Count
        {
            get
            {
                return count;
            }
        }

        /// <summary>
        ///  Creates a Custom Linked List
        ///  data: the data to add to the Custom Linked List 
        ///  default: Count = 0
        /// </summary>
        /// <param name="data">Parameter List of data to add to the Custom Linked List</param>
        public GalabingusLinkedList(params T[] data)
        {
            // Set the count to 0
            // Make the head a null pointer
            // Make the tail a null pointer
            // When there is data to add add the data to the head
            this.count = 0;
            this.head = null;
            this.tail = head;
            foreach (T d in data)
            {
                this.Add(d);
            }
        }

        /// <summary>
        ///  Adds a Node the the Custom Linked List
        ///  data: the data to be added to the Node which is added to the Custom Linked List
        ///  This Node's next will next will always be null
        /// </summary>
        /// <param name="data">data to be added to the Custom Linked List</param>
        /// <exception cref="Exception">Custom Linked List tail was not updated properly and is now null/exception>
        public void Add(T data)
        {
            // When the head is a null pointer make the head point to a
            // new node with the Data of the input data.
            // Otherwise if the tail isn't null which would only happen if in the future a method
            // inserts a node and doesn't update the tail
            // If that is all good then the tail can then be used to set the next to the new node
            // That would mean for the tail to be the new node
            if (head == null)
            {
                // Creates the new node and update the tail
                // sets the head to be the new node
                head = new GalabingusLinkedNode<T>(data);
                tail = head;
                count++;
            }
            else if (tail != null)
            {
                // Sets the new node to be the tail
                // Switches the tail to be the new created node
                tail.Next = new GalabingusLinkedNode<T>(data);
                tail = tail.Next;
                count++;
            }
            else
            {
                // A value was inserted and the tail was not updated.
                throw new Exception($"Error: Linked list tail was not updated when a value was inserted");
            }
        }

        /// <summary>
        ///  Traverses through the Custom Linked List to the given index
        ///  At the given index there are Get and Set options:
        ///  Set: sets the Custom Linked Node at the given index within the Custom Linked List
        ///  Get: retrieves a Custom Linked Node at the given index within the Custom linked List
        /// </summary>
        /// <param name="index">index of the Custom Linked Node in the Custom linked List</param>
        /// <returns>Custom Linked Node at index</returns>
        /// <exception cref="IndexOutOfRangeException">index inputted was out of the range of the Custom Linked List</exception>
        /// <exception cref="Exception">Custom Linked List was cutoff by some internal add-on method</exception>
        public T? GetData(int index)
        {
            // Check to see if the index is out of bounds
            if (index >= count || count == 0 || index < 0)
            {
                // TODO: throw out of bounds exception
                throw new IndexOutOfRangeException($"Error: Cannot get data from invalid index {index}");
            }

            // When the list is cut-off the head will be null
            if (head == null)
            {
                // The Linked List was cutoff
                // TODO: throw internal error exception
                throw new Exception($"Error: Linked list was cutoff");
            }
            else
            {
                // By logic current will never be null here
#pragma warning disable CS8602
                // Go through next Nodes until the node is the node at the index we are searching for
                GalabingusLinkedNode<T>? current = head;
                for (uint i = 0; i < index; i++)
                {
                    current = current.Next;
                }
                // Return the node's data
                return current.Data;
#pragma warning restore CS8602
            }
        }
    }
}

