using UnityEngine;
using System.IO;

public class ListRand : MonoBehaviour
{
    public ListNode head;
    public ListNode tail;
    public int count;

    private void Start() {
        TestDeserialization();
        TestSerialization();
        //DisplayList();
    }

    #region Public Methods

    public void TestDeserialization() {
        StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/listReference.txt");
        Deserialize(sr);
        sr.Close();
    }

    public void TestSerialization() {
        StreamWriter sw = new StreamWriter(Application.streamingAssetsPath + "/listReferenceWritten.txt");
        Serialize(sw);
        sw.Close();
    }

    public void Deserialize(StreamReader sr) {
        count = 0;
        ListNode previousNode = null, currentNode = null;
        string anotherNode;

        while((anotherNode = sr.ReadLine()) != null) {
            count++;
        }

        // Creating array that references to rand pointer in list
        int[] randBuffer = new int[count];
        count = 0;
        sr.BaseStream.Position = 0;
        
        while (!sr.EndOfStream) {
            anotherNode = sr.ReadLine();
            if (anotherNode != "") {
                if (head == null) {
                    head = CreateNode(anotherNode, out randBuffer[count]);
                    count++;
                    previousNode = head;
                    continue;
                }
                
                currentNode = CreateNode(anotherNode, out randBuffer[count]);


                previousNode.next = currentNode;
                currentNode.prev = previousNode;
                previousNode = currentNode;
                
                if (count == randBuffer.Length - 1) {
                    tail = currentNode;
                }
                count++;
            }
        }

        ListNode potentiallySpecifiedNode;
        currentNode = potentiallySpecifiedNode = head;
        
        // m-index provides access to randBuffer array
        // k-index provides node indexing in list for potential specified node check
        for(int k = 0, m = 0; k < randBuffer.Length; k++) {
            if (randBuffer[m] == k) {
                currentNode.rand = potentiallySpecifiedNode;

                if (currentNode.next == null) {
                    break;
                }

                currentNode = currentNode.next;
                potentiallySpecifiedNode = head;
                m++; k = -1;
                continue;
            }

            if (k == randBuffer.Length - 1) {
                // If we reached the end of randBuffer and checked all of ListNodes - stop
                if (m == k) {
                    break;
                // Otherwise take next randBuffer value and repeat
                } else { 
                    m++; k = -1;
                    currentNode = currentNode.next;
                    potentiallySpecifiedNode = head;
                    continue;
                }
            }
            // Move potential specified node along with k-index
            potentiallySpecifiedNode = potentiallySpecifiedNode.next;
        }
    }

    public void Serialize(StreamWriter sw) {
        if (head == null && tail != null) {
            head = GetHeadOfList();
        }
        
        if (head != null) { 
            ListNode currentNode = head, specifiedNode = head;
            
            // Creating array that references to rand pointer in our list
            int[] randBuffer = count > 0 ?  new int[count] : new int[GetListLenght()];

            for(int k = 0, m = 0; k < randBuffer.Length; k++) {
                if (currentNode.rand == specifiedNode) {
                    sw.WriteLine(currentNode.data + "$ " + k);

                    if (currentNode.next == null) {
                        break;
                    }

                    // Move current pointer
                    currentNode = currentNode.next; 
                    m++;
                    // Refresh check-pointer
                    specifiedNode = head; 
                    k = -1;
                    continue;
                }

                // specifiedNode.Next == null also valid condition statement but unsafe
                if (k == randBuffer.Length - 1) {
                    sw.WriteLine(currentNode.data);
                    
                    if (m == k) {
                        break;
                    } else {
                        // Move current pointer
                        currentNode = currentNode.next;
                        m++;
                        // Refresh check-pointer
                        specifiedNode = head;
                        k = -1;
                        continue;
                    }
                }
                specifiedNode = specifiedNode.next;
            }
        }
    }
        
    #endregion

    #region Private Methods

    private ListNode CreateNode(string rawTextedNode, out int randElement) {
        ListNode newNode = new ListNode();
        string[] splittedRawNode = rawTextedNode.Split('$');
        
        // If split returns more then 1 parts of primary string
        if (splittedRawNode.Length > 1) {
            try {
                newNode.data = splittedRawNode[0];
                randElement = Mathf.Abs(System.Convert.ToInt32(splittedRawNode[1]));
            }
            catch (System.FormatException) {
                newNode.data = splittedRawNode[0];
                randElement = -1;
            }
            catch (System.OverflowException) {
                newNode.data = splittedRawNode[0];
                randElement = -1;
            }
        } else {
            newNode.data = rawTextedNode;
            randElement = -1;
        }

        return newNode;
    }

    private int GetListLenght() {
        int n = 0;
        ListNode pointer = head;

        while (pointer != null) {
            n++;
            pointer = pointer.next;
        }

        return n;
    }

    private ListNode GetHeadOfList() {
        ListNode pointer = tail;

        while (pointer.next != null) {
            pointer = pointer.next;
        }

        return pointer;
    }
        
    #endregion
}
