using System.Data.Common;

public class ServerGraph {
    // 3 marks
    private class WebServer {
        public string Name;             //Unique webserver name
        public List<WebPage> P;         //What webpages this server hosts
    }

    private WebServer[] V;  //List of all webservers
    private bool[,] E;      //Edges/connections between all webservers
    private int NumServers; //Current number of servers

    // 2 marks
    // Create an empty server graph
    public ServerGraph() {
        V = new WebServer[1];   //Start at 1 so that the double capacity function doesn't have                       
        E = new bool[1, 1];     //to check if there are no elements in the array (can't multiple 0)
        NumServers = 0;    
    }

    // 2 marks
    // Return the index of the server with the given name; otherwise return -1
    private int FindServer(string name) {
        for (int i = 0; i < NumServers; i++) {
            if (name == V[i].Name) {
                //Console.WriteLine("Found server " + name);
                return i;
            }
        }
        
        //Couldn't find server
        return -1;
    }
    // 3 marks
    // Double the capacity of the server graph with respect to web servers
    private void DoubleCapacity() {
        //Double V's capacity
        //Console.WriteLine("V's capacity before: " + V.Length);
        Array.Resize(ref V, V.Length * 2);
        //Console.WriteLine("V's capacity after: " + V.Length);


        //Double E's capacity
        //Console.WriteLine("E's capacity before: " + E.GetLength(0) + "," + E.GetLength(1));
        int currentRows = E.GetLength(0);
        int currentColumns = E.GetLength(1);

        //Create a new matrix with double the capacity
        bool[,] newMatrix = new bool[currentRows * 2, currentColumns * 2];

        //Copy elements from the old matrix to the new one
        for (int i = 0; i < currentRows; i++) {
            for (int j = 0; j < currentColumns; j++) {
                newMatrix[i, j] = E[i, j];
            }
        }

        // Update the reference to the new matrix
        E = newMatrix;
        //Console.WriteLine("E's capacity after: " + E.GetLength(0) + "," + E.GetLength(1));
    }

    // 3 marks
    // Add a server with the given name and connect it to the other server
    // Return true if successful; otherwise return false
    public bool AddServer(string name, string other) {
        //Determine if this is the first server being added
        if (NumServers == 0)
        {
            //Create a new webserver
            var newWebServer = new WebServer {
                Name = name
            };

            V[NumServers] = newWebServer;

            //Set adjacency matrix
            E[0,0] = false;    

            //Increment after adding
            NumServers++;

            Console.WriteLine("This was the first server added. " + name + " has been added with no connections.");
            return true;
        }
        //This is the not the first server being added
        else {
            //If the server exists
            if (FindServer(name) != -1) {
                Console.WriteLine("Server " + name + " already exists.");
                return false;
            }

            //If the other server doesn't exist
            if (FindServer(other) == -1) {
                Console.WriteLine("Other server " + other +" not found when trying to add new webserver " + name);
                return false;
            }

            //Check if the capacity needs to be increased
            if ((NumServers + 1) > V.Length) { 
                DoubleCapacity();
            }

            //Create a new webserver
            var newWebServer = new WebServer
            {
                Name = name
            };

            V[NumServers] = newWebServer;

            //Initialize all of the edges for this new vertex to false (no edges/connections)
            //We don't need 2 loops b/c we can just swap the order of i and numServers
            for (int i = 0; i <= NumServers; i++) {
                E[NumServers, i] = false;     //say numVertices = 4  then the iterations would look like this (4,0),(4,1),(4,2),(4,3),(4,4) were everyone would be set to -1
                E[i, NumServers] = false;     //(0,4),(1,4),(2,4),(3,4),(4,4)
            }

            //Increment after adding
            NumServers++;

            //Connect it to the other server in the adjacency matrix E
            //The ServerGraph is undirected, so set the adjacency matrix both ways
            var otherIndex = FindServer(other);
            var nameIndex = FindServer(name);
            //Console.WriteLine(nameIndex);
            //Console.WriteLine(otherIndex);
            E[nameIndex,otherIndex] = true;
            E[otherIndex,nameIndex] = true;

            return true;
        }
    }

    // 3 marks
    // Add a webpage to the server with the given name
    // Return true if successful; other return false
    //public bool AddWebPage(WebPage w, string name) {
        //return false;
    //}

    // 4 marks
    // Remove the server with the given name by assigning its connections
    // and webpages to the other server
    // Return true if successful; otherwise return false
    public bool RemoveServer(string name, string other) {
        return false;
    }
    // 3 marks
    // Add a connection from one server to another
    // Return true if successful; otherwise return false
    // Note that each server is connected to at least one other server
    public bool AddConnection(string from, string to) {
        return false;
    }
    // 10 marks
    // Return all servers that would disconnect the server graph into
    // two or more disjoint graphs if ever one of them would go down
    // Hint: Use a variation of the depth-first search
    public string[] CriticalServers() {
        string[] criticalServers = new string[20];
        
        //


        return criticalServers;

    }
    // 6 marks
    // Return the shortest path from one server to another
    // Hint: Use a variation of the breadth-first search
    public int ShortestPath(string from, string to) {
        return -1;
    }
    
    // 4 marks
    // Print the name and connections of each server as well as
    // the names of the webpages it hosts
    public void PrintGraph() { 
        Console.WriteLine("Number of servers: " + NumServers);


        Console.WriteLine("WebServers Length:" + V.Length);
        for (int i = 0; i < NumServers; i++) {
            Console.WriteLine(V[i].Name);
        }

        //Start at -1 so that I can print the names for the columns (probably a better way to do this)
        //Edges:
        //        A(0)    B(1)    D(2)    E(3)    F(4)    G(5)      <-- else does this
        //A(0)    False   True    False   True    True    True
        //B(1)    True    False   True    False   False   False
        //D(2)    False   True    False   False   False   False
        //E(3)    True    False   False   False   False   False
        //F(4)    True    False   False   False   False   False
        //G(5)    True    False   False   False   False   False
        Console.WriteLine("Edges:");
        for (int row = -1; row < NumServers; row++) {
            //from 0 to numServers, print out the row names and the E[] values
            if (row != -1) {
                Console.Write(V[row].Name + "(" + row + ")" + "    ");

                for (int column = 0; column < NumServers; column++) {
                    //Write the edge value
                    Console.Write(E[row, column] + "    ");
                }
            }
            //Write the column names for the very top row
            else {
                var msg = "         ";
                for (int i = 0; i < NumServers; i++) {
                    msg = msg + V[i].Name + "(" + i + ")" + "    ";
                }
                Console.Write(msg);
            }
            
            //Start the next line
            Console.WriteLine("");
        }

        //Print webpages hosted by each server
        Console.WriteLine("Webpages hosted by servers");
    }
}
// 5 marks
class WebPage {
    public string Name { get; set; }
    public string Server { get; set; }
    public List<WebPage> E { get; set; }
   

    public WebPage(string name, string host) {
        Name = name;
        Server = host;
        E = new List<WebPage>();
    }

    public int FindLink(string name) {
        return -1;
    }
    

}
class WebGraph {
    private List<WebPage> P;
    //...

    // 2 marks
    // Create an empty WebGraph
    public WebGraph() { 
    
    }

    // 2 marks
    // Return the index of the webpage with the given name; otherwise return -1
    private int FindPage(string name) {
        return -1;
    }

    // 4 marks
    // Add a webpage with the given name and store it on the host server
    // Return true if successful; otherwise return false
    public bool AddPage(string name, string host) {
        return false;
    }

    // 8 marks
    // Remove the webpage with the given name, including the hyperlinks
    // from and to the webpage
    // Return true if successful; otherwise return false
    public bool RemovePage(string name) {
        return false;
    }

    // 3 marks
    // Add a hyperlink from one webpage to another
    // Return true if successful; otherwise return false
    public bool AddLink(string from, string to) {
        return false;
    }

    // 3 marks
    // Remove a hyperlink from one webpage to another
    // Return true if successful; otherwise return false
    public bool RemoveLink(string from, string to) {
        return false;
    }

    // 6 marks
    // Return the average length of the shortest paths from the webpage with
    // given name to each of its hyperlinks
    // Hint: Use the method ShortestPath in the class ServerGraph
    public float AvgShortestPaths(string name) { 
        return 0f;
    }

    // 3 marks
    // Print the name and hyperlinks of each webpage
    public void PrintGraph() {
    ///////////////////////////////// maybe something like this
    ///Node  Connected(c)   Node
    ///Node A connections
    /// A        False       A
    ///A        True        B
    ///A        False       C
    ///
    ///Node C
    ///C        False       A
    }
}


class Program
{
    static void Main(string[] args)
    {
        //Console.WriteLine("Hello, World!");

        ////////////////Testing//////////////////////////////////
        //1.Instantiate a server graph and a web graph.
        var serverGraphOne = new ServerGraph();
        //Console.WriteLine("New ServerGraph created: " + serverGraphOne);

        //2.Add a number of servers.
        serverGraphOne.AddServer("A", "B"); //Test adding the first server

        serverGraphOne.AddServer("B", "A"); //Test Connecting to an existing server

        serverGraphOne.AddServer("C", "Z"); //Test Connecting to a non existing server

        serverGraphOne.AddServer("B", "A"); //Test Adding a duplicate server

        serverGraphOne.AddServer("D", "B"); //Test increasing the capacity
        serverGraphOne.AddServer("E", "A"); 
        serverGraphOne.AddServer("F", "A"); 
        serverGraphOne.AddServer("G", "A"); 

        serverGraphOne.PrintGraph();

        //3.Add additional connections between servers.
        //4.Add a number of webpages to various servers.
        //5.Add and remove hyperlinks between the webpages.
        //6.Remove both webpages and servers.
        //7.Determine the articulation points of the remaining Internet.
        //8.Calculate the average shortest distance to the hyperlinks of a given webpage.


        //In a separate document, illustrate your test cases and compare them with the results of your program.
    }
}