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
                    throw new IndexOutOfRangeException($"Error: Cannot get data from invalid index {index}.");
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
                    throw new IndexOutOfRangeException($"Error: Cannot set data at invalid index {index}.");
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
        public int Count
        {
            get
            {
                return (int)count;
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
                head.Previous = null;
                head.Next = null;
                tail = head;
                count++;
            }
            else if (tail != null)
            {
                // Sets the new node to be the tail
                // Switches the tail to be the new created node
                GalabingusLinkedNode<T> itemNode = new GalabingusLinkedNode<T>(data);
                itemNode.Next = tail.Next;
                itemNode.Previous = tail;
                tail.Next = itemNode;
                tail = tail.Next;
                count++;
            }
            else
            {
                // A value was inserted and the tail was not updated.
                throw new Exception($"Error: Linked list tail was not updated when a value was inserted.");
            }
        }

        /// <summary>
        ///  Removes an item at a given index from the Custom Linked List
        ///  Returns the data inside the Node at the index of removal
        /// </summary>
        /// <param name="index">index of the Custom Linked Node in the Custom linked List</param>
        /// <returns>Custom Linked Node's data at index of removal</returns>
        /// <exception cref="IndexOutOfRangeException">index inputted was out of the range of the Custom Linked List</exception>
        public T? RemoveAt(int index)
        {
            // Check to see if the index is out of bounds
            if (index >= count || count == 0 || index < 0)
            {
                throw new IndexOutOfRangeException($"Error: Cannot remove invalid index {index}.");
            }

            // The index is the head so change the head by setting it to the head's next
            if (index == 0)
            {
                // By logic current will never be null here
#pragma warning disable CS8602
                // Store the node that will be removed
                T? tempData = head.Data;
                if (count == 1)
                {
                    head = null;
                    count--;
                    return tempData;
                }
                head = head.Next;
                head.Previous = null;
#pragma warning restore CS8602
                count--;
                return tempData;
            }

            // The index is the last node, change the tail and set the next to null
            if (index == (count - 1))
            {
                // By logic current will never be null here
#pragma warning disable CS8602
                T? tempData = tail.Data;
                tail = tail.Previous;
                tail.Next = null;
#pragma warning restore CS8602
                count--;
                // Return the node's data
                return tempData;
            }

            // The index is somewhere in the middle of the Linked List
            // Removal is done by setting the Node at the index - 1's next to the next.next
            {
                // By logic current will never be null here
#pragma warning disable CS8602
                // Go through next Nodes until the node is the node at the index we are searching for
                GalabingusLinkedNode<T>? current = head;
                for (uint i = 0; i < index - 1; i++)
                {
                    current = current.Next;
                }
                T? tempData = current.Next.Data;
                current.Next.Next.Previous = current;
                current.Next = current.Next.Next;
                count--;
                // Return the node's data
                return tempData;
#pragma warning restore CS8602
            }
        }

        public void Insert(T item, int index)
        {
            // Check to see if the index is out of bounds
            if (index >= count + 1 && index != 0 || count == 0 && index != 0 || index < 0)
            {
                // TODO: throw out of bounds exception
                throw new IndexOutOfRangeException($"Error: Cannot insert into invalid index {index}.");
            }

            // The index is the head so change the head and when at count == 1 the tail
            if (index == 0)
            {
                // By logic current will never be null here
#pragma warning disable CS8602
                // Store the node that will be removed
                GalabingusLinkedNode<T> itemNode = new GalabingusLinkedNode<T>(item);
                if (head == null)
                {
                    head = itemNode;
                    tail = head;
                    count++;
                    return;
                }
                itemNode.Next = head;
                head.Previous = itemNode;
                head = itemNode;
#pragma warning restore CS8602
                count++;
                return;
            }

            // The index is the last node, change the tail
            if (index == (count - 1))
            {
                // By logic current will never be null here
#pragma warning disable CS8602
                GalabingusLinkedNode<T> itemNode = new GalabingusLinkedNode<T>(item);
                itemNode.Next = tail;
                itemNode.Previous = tail.Previous;
                tail = itemNode.Next;
                tail.Previous = itemNode;
#pragma warning restore CS8602
                count++;
                return;
            }

            // The index is the size of the count, so add the node to the end
            if (index == count)
            {
                // By logic current will never be null here
#pragma warning disable CS8602
                GalabingusLinkedNode<T> itemNode = new GalabingusLinkedNode<T>(item);
                tail.Next = itemNode;
                itemNode.Previous = tail;
                tail = itemNode;
#pragma warning restore CS8602
                count++;
                return;
            }

            // The index is somewhere in the middle of the Linked List
            {
                // By logic current will never be null here
#pragma warning disable CS8602
                // Go through next Nodes until the node is the node at the index we are searching for
                GalabingusLinkedNode<T>? current = head;
                for (uint i = 0; i < index - 1; i++)
                {
                    current = current.Next;
                }
                GalabingusLinkedNode<T> itemNode = new GalabingusLinkedNode<T>(item);
                itemNode.Next = current.Next;
                itemNode.Previous = current.Next.Previous;
                current.Next.Previous = itemNode;
                current.Next = itemNode;
                count++;
                return;
#pragma warning restore CS8602
            }
        }

        /// <summary>
        ///  Clears the Custom Linked List
        ///   Sets the head and tail pointer null
        ///   Resets the count to 0
        /// </summary>
        public void Clear()
        {
            head = null;
            tail = null;
            count = 0;
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

        /// <summary>
        ///  Writes all Nodes Data to the console
        ///  Starts at the tail goes to the head
        /// </summary>
        public void PrintBackward()
        {
            string result = "";
#pragma warning disable CS8602
            // Go through previous Nodes concatenating all of the data to the result
            GalabingusLinkedNode<T>? current = tail;
            for (uint i = 0; i < count; i++)
            {
                if (current != null && (i + 1) < count)
                {
                    result = result + current.Data + "\n";
                    current = current.Previous;
                }
                else if (current != null)
                {
                    result = result + current.Data;
                    current = current.Previous;
                }

            }
            // Only show the result when there actually is info to show
            if (count == 0)
            {
                Console.WriteLine("There are no items in the list.");
            }
            else
            {
                Console.WriteLine(result);
            }
#pragma warning restore CS8602
        }


        /// <summary>
        ///  Writes all Nodes Data to the console
        ///  Starts at the head goes to the tail
        /// </summary>
        public void PrintForward()
        {
            string result = "";
#pragma warning disable CS8602
            // Go through next Nodes concatenating all of the data to the result
            GalabingusLinkedNode<T>? current = head;
            for (uint i = 0; i < count; i++)
            {
                if (current != null && (i + 1) < count)
                {
                    result = result + current.Data + "\n";
                    current = current.Next;
                }
                else if (current != null)
                {
                    result = result + current.Data;
                    current = current.Next;
                }

            }
#pragma warning restore CS8602
            // Only show the result when there actually is info to show
            if (count == 0)
            {
                Console.WriteLine("There are no items in the list.");
            }
            else
            {
                Console.WriteLine(result);
            }
        }
    }
}

