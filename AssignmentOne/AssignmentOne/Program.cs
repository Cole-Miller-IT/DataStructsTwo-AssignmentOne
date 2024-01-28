using System.Data.Common;
using System.Runtime.InteropServices;
using System.Xml.Linq;

public class ServerGraph {
    // 3 marks 
    private class WebServer {
        public string Name = "";        //Unique webserver name
        public List<WebPage> P = new List<WebPage>();         //What webpages this server hosts
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

    // 2 marks DONE
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

    // 3 marks DONE
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

    // 3 marks DONE
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
            var newWebServer = new WebServer {
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
            var otherIndex = FindServer(other);
            var nameIndex = FindServer(name);
            //Console.WriteLine(nameIndex);
            //Console.WriteLine(otherIndex);
            E[nameIndex,otherIndex] = true; //The ServerGraph is undirected, so set the adjacency matrix both ways
            E[otherIndex,nameIndex] = true;

            return true;
        }
    }

    // 3 marks DONE
    // Add a webpage to the server with the given name
    // Return true if successful; other return false
    public bool AddWebPage(WebPage w, string serverName) {
        //Check if server exists
        var serverNameIndex = FindServer(serverName);
        if (serverNameIndex == -1) {
            Console.WriteLine("Server " + serverName + " does not exist. Cannot add webpage.");
            return false;
        }

        //Add the webpage to the server
        V[serverNameIndex].P.Add(w);
        return true;
    }

    //DONE
    //Function that I created
    //Called by the web graphs RemoveWebpage() method to remove the webpage from the server after the hyperlinks pointing to it have been removed.
    //This shouldn't be public because it just supports the removeWebpage() method in the webgraph class, calling it on it's own would cause issues.
    //I don't know what access modifier to use.
    public bool RemoveWebPage(WebPage w) {
        var hostIndex = FindServer(w.Server);
        var result = V[hostIndex].P.Remove(w);

        if (result == true) {
            return true;
        }
        else {
            Console.WriteLine("Unable to remove webpage " + w.Name + " from server " + w.Server);
            return false;
        }
    }

    // 4 marks DONE
    // Remove the server with the given name by assigning its connections
    // and webpages to the other server
    // Return true if successful; otherwise return false
    public bool RemoveServer(string name, string other) {
        var nameIndex = FindServer(name);
        var otherIndex = FindServer(other);
        
        //If servers don't exist
        if (nameIndex == -1) {
            Console.WriteLine("First server does not exist " + name + ". Can't remove server.");
            return false;
        }
        
        if (otherIndex == -1) {
            Console.WriteLine("Other server does not exist " + other + ". Can't remove server.");
            return false;
        }

        //Check if the other server is the same as the name server
        if (name == other) {
            Console.WriteLine("Cannot remove " + name + ". " + name + " specified as both name and other server.");
            return false;
        }

        //Move over all of the webpages currently on the old server. *NOTE: "name" server still points to the webpages but will be deleted later, so we don't have to remove name's references.
        for (int i = 0; i < V[nameIndex].P.Count; i++) {
            var webpage = V[nameIndex].P[i];    //Gets a reference to the webpage we are moving over
            //Change the webpages host names
            webpage.Server = V[otherIndex].Name;

            V[otherIndex].P.Add(webpage);       //Have the new server point towards the webpage
        }

        NumServers--;

        //We want to replace the values of the removed vertex with the last vertex's adjancency data
        V[nameIndex] = V[NumServers]; //The vertex index of the vertex we are removing is now equal to the last vertex in the list

        //This will loop through all of the values in the adjacency matrix to replace the values 
        for (int j = NumServers; j >= 0; j--) {    //Say numVertices = 4, and vertexIndex = 1
            E[j, nameIndex] = E[j, NumServers];  //(4,1)'s value replaced with (4,4)'s value (value being the weight assigned to that edge)
            E[nameIndex, j] = E[NumServers, j];  //(1,4)'s value replaced with (4,4)'s value (value being the weight assigned to that edge)
                                                    //Then  (3,1) replaced with (3,4)
                                                    //      (1,3) replaced with (4,3) and so on...
        }

        return true;
    }

    // 3 marks DONE
    // Add a connection from one server to another
    // Return true if successful; otherwise return false
    // Note that each server is connected to at least one other server
    public bool AddConnection(string from, string to) {
        var fromServerIndex = FindServer(from);
        var toServerIndex = FindServer(to);

        //Check if either server doesn't exist
        if (fromServerIndex == -1) {
            Console.WriteLine("Server " + from + " does not exist. Cannot connect.");
            return false;
        }

        if (toServerIndex == -1) {
            Console.WriteLine("Server " + to + " does not exist. Cannot connect.");
            return false;
        }

        //Both servers exist, add a connection between them because the serverGraph is undirected
        E[fromServerIndex, toServerIndex] = true;
        E[toServerIndex, fromServerIndex] = true;

        return true;
    }
    
    // 10 marks DONE
    // Return all servers that would disconnect the server graph into
    // two or more disjoint graphs if ever one of them would go down
    // Hint: Use a variation of the depth-first search
    public string[] CriticalServers() {
        string[] criticalServers = new string[NumServers];  //NumServers length will gurantee that the array will hold all of the potential critical servers
    
        //For all of the servers in the graph
        for (int i = 0; i < NumServers; i++) {
            //Get the current server
            var currentServer = V[i];

            //Get one of its neighbors
            var currentServersNeighbor = GetNeighbor(currentServer.Name);

            Console.WriteLine("Current Server: " + currentServer.Name);
            Console.WriteLine("Nearest Neighbor: " + currentServersNeighbor.Name);

            //Remove the connections from the current server, but remember them for later.  //#NOTE, could change this later to only copy the rows and columns of the current server for scalability.
            bool[,] originalE = new bool[E.GetLength(0), E.GetLength(0)];
            Array.Copy(E, originalE, E.Length); //Create a deep copy of E

            var tempE = RemoveConnections(currentServer.Name);
            E = tempE;

            //Perform a depth-first search on the neighbor
            var visitedServers = DepthFirstSearch(currentServersNeighbor.Name);
            Console.WriteLine("Visited server count: " + visitedServers);

            //If the result of the depth-first search can't now find every other server then the removed server was a critical server
            //Add it to the list of critical servers
            //NumServers - 1 because we "removed" the current server
            if (visitedServers != (NumServers - 1)) {
                //current server was a critical server
                criticalServers[i] = currentServer.Name;
                Console.WriteLine("Critical server found: " + currentServer.Name);
            }

            //Add back the connections we removed earlier
            E = originalE;

            Console.WriteLine();
        }

        return criticalServers;
    }

    //DONE
    //Supporting method for CriticalServers(). Gets the first neighbor from the the current server.
    private WebServer GetNeighbor(string server) {
        int serverIndex = FindServer(server);

        //Loop though the row of the current server
        //i.e. A    B 
        //A  false  true
        //B   true  false   <-- this row until a nieghbor is found (i.e. true is found)
        //E[servIndex, 0], E[servIndex, 1], ...
        for (int i = 0; i < NumServers; i++) {
            //If we found an edge
            if (E[serverIndex, i] == true) {
                //Return the nieghbor
                return V[i];
            }
        }

        return null;
    }

    //DONE
    // Supporting method for CriticalServers(). Removes connections from the current server.
    private bool[,] RemoveConnections(string serverName) {
        var tempE = E;   //Stores the edges from E, with all edges to the current server removed

        int serverIndex = FindServer(serverName);

        //Loop through to remove all possible edges
        for (int i = 0; i < NumServers; i++) {
            E[serverIndex, i] = false;        // Remove row connections
            E[i, serverIndex] = false;        // Remove column connections
        }

        return tempE;
    }

    //DONE
    //Supporting method for CriticalServers(). Modified version of DFS that returns the number of visited nodes.
    private int DepthFirstSearch(string neighborsName) {
        //We want to traverse the tree by going as far down as we can before going back up again. And this process
        //will repeat recursively until all nodes have been found
        bool[] visitedNodes = new bool[NumServers];
        int index = FindServer(neighborsName);

        //Clear the array
        for (int i = 0; i < NumServers; i++) {
            visitedNodes[i] = false;
        }

        //Call DFS only on the neighbor
        visitedNodes = DepthFirstSearchPrivate(index, visitedNodes);

        //See how many nodes were visited by the DFS
        int count = 0;
        for (int i = 0; i < visitedNodes.Length; i++) {
            if (visitedNodes[i] == true) {
                count++;
            }
        }

        return count;
    }

    //DONE
    //Supporting method for DepthFirstSearch(). Returns the array of visited nodes
    private bool[] DepthFirstSearchPrivate(int i, bool[] visitedNodes) {
        //Recursion is a little hard to think about for me in these examples so imagine it like this
        //you arrive at node A, you change the visitedNodes list to true, then immediately call this function on B.
        //              A --> B
        int j;
        visitedNodes[i] = true;    // Output vertex when marked as visited
        Console.WriteLine(i + "(" + V[i].Name + ")");

        // Visit next unvisited adjacent vertex, j < numServers b/c this node could potentially be connected to every other node
        for (j = 0; j < NumServers; j++) {
            //This checks if we have visited the node && if there is an edge connected to this node
            if (!visitedNodes[j] && E[i, j] == true) {
                //This current node has an edge connected to an unvistited node, call DepthFirst search function again to traverse it
                DepthFirstSearchPrivate(j, visitedNodes);
            }
        }

        return visitedNodes;
    }

    // 6 marks DONE
    // Return the shortest path from one server to another
    // Hint: Use a variation of the breadth-first search
    public int ShortestPath(string from, string to) {
        var fromIndex = FindServer(from);
        var toIndex = FindServer(to);

        //If the servers don't exist
        if (fromIndex == -1) {
            return -1;
        }
        if (toIndex == -1) {
            return - 1;
        }

        //Distance for a server to itself
        if (from == to) {
            return 0;
        }

        bool[] visitedServers = new bool[NumServers];
        int[] distances = new int[NumServers]; // Array to store distances from the starting server


        //Start at the "from" server and perform a breadth-first search looking for "to"
        var pathDistance = ShortestPathPrivate(fromIndex, visitedServers, to, distances);
        return pathDistance;
    }

    //Supporting method for the public ShortestPath() method
    private int ShortestPathPrivate(int fromIndex, bool[] visitedNodes, string to, int[] distances) {
        int i;
        int j;
        Queue<int> Q = new Queue<int>();

        //Add our start server to the queue to begin the breadth-first search
        Q.Enqueue(fromIndex);
        visitedNodes[fromIndex] = true;
        distances[fromIndex] = 0; // Distance from the starting server to itself is 0

        while (Q.Count != 0) {
            i = Q.Dequeue();

            //If we found the server
            if (V[i].Name == to) {
                return distances[i]; // Return the distance when the destination server is found
            }

            //Look through all of the servers to check if they are neighbors with the current server we are looking at
            for (j = 0; j < NumServers; j++){
                //If this is a neighbor of the node we are currently at
                if (!visitedNodes[j] && E[i, j]) {
                    Q.Enqueue(j);
                    visitedNodes[j] = true;
                    distances[j] = distances[i] + 1; // Update the distance for the adjacent node
                }
            }
        }

        return -1; // Return -1 if the destination server is not reachable
    }

    // 4 marks DONE
    // Print the name and connections of each server as well as
    // the names of the webpages it hosts
    public void PrintGraph() {
        Console.WriteLine();
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
        //Loop through each of the servers 
        for (int i = 0; i < NumServers; i++) {
            Console.Write("Server " + V[i].Name + " is hosting the following webpages: ");
            //Loop through all of the webpages for the current server
            string msg = "";
            for (int j = 0; j < V[i].P.Count; j++) {
                //Concatenate all of the webpages names to print out later
                msg = msg + V[i].P[j].Name + ", ";
            }
            Console.Write(msg);
            Console.WriteLine();
        }

        Console.WriteLine();
    }
}


// 5 marks DONE?
public class WebPage {
    public string Name { get; set; }
    public string Server { get; set; }
    public List<WebPage> E { get; set; } //These are the hyperlinks to other webpages  e.g steam.com("this" webpage) --> (points to)steam.ca
   

    public WebPage(string name, string host) {
        Name = name;
        Server = host;
        E = new List<WebPage>();
    }

    public int FindLink(string name) {
        //Search through all of the links that this webpage has
        for (int i = 0; i < E.Count; i++) {
            if (name == E[i].Name) {
                Console.WriteLine("Found link");
                return i;
            }
        }
        
        return -1;
    }
}


class WebGraph {
    private List<WebPage> P;
    private ServerGraph serverGraph;    //#NOTE: Made the WebGraph dependant on a ServerGraph already existing. Doesn't really make sense to have a WebGraph without a ServerGraph b/c 
                                        //the webpages need a host name.

    // 2 marks DONE
    // Create an empty WebGraph
    public WebGraph(ServerGraph sg) {
        P = new List<WebPage>();
        serverGraph = sg;
    }


    // 2 marks DONE
    // Return the index of the webpage with the given name; otherwise return -1
    private int FindPage(string name) {
        //Look through all of the webpages in the list P
        for (int i = 0; i < P.Count; i++) {
            //if the name is found
            if (name == P[i].Name) {
                return i;
            }
        }

        return -1;
    }


    // 4 marks DONE
    // Add a webpage with the given name and store it on the host server
    // Return true if successful; otherwise return false
    public bool AddPage(string name, string host) {
        var newWebpage = new WebPage(name, host);

        //Page doesn't exist
        if (FindPage(newWebpage.Name) == -1) {
            //Add the webpage to the server
            var result = serverGraph.AddWebPage(newWebpage, newWebpage.Server);

            //if true, then the server host exists. The webpage has just been added to the server host, now we update the P listing in this webgraph.
            if (result == true) {
                //Add the webpage to the web graph
                P.Add(newWebpage);
                return true;
            }
            else {
                Console.WriteLine("Error adding webpage " + name + ": Host server probably doesn't exist.");
                return false;
            }
        }
        //Page exists
        else {
            Console.WriteLine("Page already exists");
            return false;
        }
    }

    // 8 marks DONE
    // Remove the webpage with the given name, including the hyperlinks
    // from and to the webpage
    // Return true if successful; otherwise return false
    public bool RemovePage(string name) {
        var pageToRemoveIndex = FindPage(name);

        //If the page doesn't exist
        if (pageToRemoveIndex == -1) {
            Console.WriteLine("The page (" + name + ") you are trying to remove does not exist.");
            return false;
        }

        //We have to remove anything that points/references the webpage. i.e. any hyperlinks from oher webpages to the one you are trying to remove.
        //Loop through all of the webpages in P from the webgraph
        for (int i = 0; i < P.Count; i++) {
            //Loop through all of the hyperlinks that the current webpage has
            for (int j = 0; j < P[i].E.Count; j++) {
                if (P[i].E[j].Name == name) {
                    Console.WriteLine("Hyperlink found referencing " + name + " in webpage " + P[i].Name);

                    //Remove the hyperlink
                    P[i].E.RemoveAt(j);
                }
            }
        }

        //After removing all of the hyperlinks that reference our webpage, remove it from the webpage's host server
        var result = serverGraph.RemoveWebPage(P[pageToRemoveIndex]);

        //Remove the webpage from the webGraph
        P.RemoveAt(pageToRemoveIndex);

        if (result == true) {
            return true;
        }
        else {
            return false;
        }
    }

    // 3 marks DONE
    // Add a hyperlink from one webpage to another
    // Return true if successful; otherwise return false
    public bool AddLink(string from, string to) {
        var fromIndex = FindPage(from);
        var toIndex = FindPage(to);

        //If webpages don't exist
        if (fromIndex == -1) {
            Console.WriteLine("webpage " + from + " does not exist. Cannot add link.");
            return false;
        }

        if (toIndex == -1) {
            Console.WriteLine("webpage " + to + " does not exist. Cannot add link.");
            return false;
        }

        //Add a check here if a webpage shouldn't link to itself
        //if (from == to) {
        //return false;
        //}

        //Check if there is already a hyperlink from    from --> to
        for (int i = 0; i < P[fromIndex].E.Count; i++) {
            //If there was a connection found
            if (P[fromIndex].E[i] == P[toIndex]) {
                Console.WriteLine(from + " already has a connection to " + to + ". Only one edge is allowed going to a webpage.");
                return false;
            }
        }

        //If the webpages both exist, link the "from" webpage to the "to" webpage. The webgraph is bidirectional, so not the same as the serverGraph
        P[fromIndex].E.Add(P[toIndex]);
        return true;
    }

    // 3 marks DONE
    // Remove a hyperlink from one webpage to another
    // Return true if successful; otherwise return false
    public bool RemoveLink(string from, string to) {
        var fromIndex = FindPage(from);
        var toIndex = FindPage(to);

        //If webpages don't exist
        if (fromIndex == -1) {
            Console.WriteLine("webpage " + from + " does not exist. Cannot remove link.");
            return false;
        }

        if (toIndex == -1) {
            Console.WriteLine("webpage " + to + " does not exist. Cannot remove link.");
            return false;
        }

        //Find the connection from "from" --> "to"
        for (int i = 0; i < P[fromIndex].E.Count; i++) {
            //If a connection is found
            if (P[fromIndex].E[i] == P[toIndex]) {
                //Remove the link from "from" that points to "to"
                P[fromIndex].E.RemoveAt(i);
                return true;
            }
        }
        Console.WriteLine("Error: Removing hyperlink from webpage " + from + " to " + to);
        return false;
    }

    // 6 marks DONE
    // Return the average length of the shortest paths from the webpage with
    // given name to each of its hyperlinks
    // Hint: Use the method ShortestPath in the class ServerGraph
    public float AvgShortestPaths(string webpageName) {
        //if webpage is not valid
        var webpageIndex = FindPage(webpageName);
        if (webpageIndex == -1) {
            Console.WriteLine("Page not real");
            return -1f;
        }

        var shortestPath = 0;
        //for all hyperlinks that this webpage has
        for (int i = 0; i < P[webpageIndex].E.Count; i++) {
            var hyperlinksServerHost = P[webpageIndex].E[i].Server;
            Console.WriteLine(hyperlinksServerHost);

            //Perform a BFS on the webpage's host server to the hyperlink's host server, get back the distance between those servers
            var hyperlinkShortestPath = serverGraph.ShortestPath(P[webpageIndex].Server, hyperlinksServerHost);
            Console.WriteLine("Shortest Path from " + P[webpageIndex].Server + " to " + hyperlinksServerHost + " is: " + hyperlinkShortestPath);

            shortestPath = shortestPath + hyperlinkShortestPath;
        }

        //Calculate the average path cost
        //Console.WriteLine("Shortest Path found end: " + shortestPath);
        //Console.WriteLine("E count: " + P[webpageIndex].E.Count);
        float avgShortestPath = shortestPath / P[webpageIndex].E.Count;
        Console.WriteLine("Average Shortest Path found: " + avgShortestPath + " for webpage " + webpageName);
        Console.WriteLine("-----------------------------");
        return avgShortestPath;
    }

    // 3 marks DONE
    // Print the name and hyperlinks of each webpage
    public void PrintGraph() {
        for (int i = 0; i < P.Count; i++) {
            Console.WriteLine();
            Console.WriteLine(P[i].Name + " connected to " + P[i].Server);

            Console.Write(P[i].Name + " has the following hyperlinks: ");
            //Print Hyperlinks
            string msg = "";
            //Go through the list of webpages
            for (int j = 0; j < P[i].E.Count; j++) {
                //Concatenate all of the hyperlink names to print out later
                msg = msg + P[i].E[j].Name + ", ";
            }
            Console.Write(msg);
            Console.WriteLine();
        }
    }
}


class Program {
    static void Main(string[] args) {
        ////////////////Testing//////////////////////////////////
        //1.Instantiate a server graph and a web graph.
        var serverGraphOne = new ServerGraph();
        var webGraphOne = new WebGraph(serverGraphOne);

        //Console.WriteLine("New ServerGraph created: " + serverGraphOne);

        //2.Add a number of servers.
        Console.WriteLine("Testing addServers()");
        serverGraphOne.AddServer("A", "B"); //Test adding the first server

        serverGraphOne.AddServer("B", "A"); //Test Connecting to an existing server

        serverGraphOne.AddServer("C", "Z"); //Test Connecting to a non existing server

        serverGraphOne.AddServer("B", "A"); //Test Adding a duplicate server

        serverGraphOne.AddServer("D", "B"); //Test increasing the capacity
        serverGraphOne.AddServer("E", "A"); 
        serverGraphOne.AddServer("F", "A"); 
        serverGraphOne.AddServer("G", "A"); 

        //serverGraphOne.PrintGraph();

        //3.Add additional connections between servers.
        Console.WriteLine("Testing addConnections().");
        serverGraphOne.AddConnection("E", "F"); //Test adding a connection between two existing servers

        serverGraphOne.AddConnection("E", "Z"); //Test adding a connection between one existing and one non-existing server

        serverGraphOne.AddConnection("E", "F"); //Test adding a connection between two existing servers that already have a connection

        serverGraphOne.AddConnection("Z", "X"); //Test adding a connection between two non-existing servers

        //serverGraphOne.PrintGraph();


        Console.WriteLine("Testing addPages()");
        //4.Add a number of webpages to various servers.
        webGraphOne.AddPage("001", "B");    //Test adding a webpage to an existing host

        webGraphOne.AddPage("002", "Z");    //Test adding a webpage to a non-existing host

        webGraphOne.AddPage("001", "B");    //Test adding a duplicate webpage to an existing host

        webGraphOne.AddPage("001", "Z");    //Test adding a duplicate webpage to a non-existing host

        webGraphOne.AddPage("003", "A");    //Test adding multiple webpages
        webGraphOne.AddPage("004", "A");    
        webGraphOne.AddPage("005", "B");    
        webGraphOne.AddPage("006", "B");    
        webGraphOne.AddPage("007", "D");   
        webGraphOne.AddPage("008", "D");    


        //webGraphOne.PrintGraph();
        //serverGraphOne.PrintGraph();        //Testing printGraph()



        //5.Add and remove hyperlinks between the webpages.
        webGraphOne.AddLink("003", "004");  //Test adding a hyperlink between two existing webpages

        webGraphOne.AddLink("003", "zzz");  //Test adding a hyperlink from an existing webpage to a non-existing webpage

        webGraphOne.AddLink("zzz", "004");  //Test adding a hyperlink from a non-existing webpage to an existing webpage
        
        webGraphOne.AddLink("zzx", "zzz");  //Test adding a hyperlink two nonexisting webpages
        
        webGraphOne.AddLink("003", "004");  //Test adding a duplicate hyperlink

        webGraphOne.AddLink("004", "003");  //Test adding a hyperlink in the other direction

        webGraphOne.AddLink("004", "007");  //Test adding multiple hyperlinks
        webGraphOne.AddLink("004", "008");  
        webGraphOne.AddLink("008", "001");  

        webGraphOne.AddLink("003", "003");  //Test connecting a webpage to itself
        webGraphOne.AddLink("003", "003");  //Test connecting a webpage to itself again


        webGraphOne.RemoveLink("003", "003");       //Test removing an existing link

        webGraphOne.RemoveLink("003", "zzz");       //Test removing a non existing link

        webGraphOne.RemoveLink("zzz", "003");       //Test removing from a non-existing webpage

        webGraphOne.RemoveLink("004", "003");       //Test removing an existing link

        //webGraphOne.PrintGraph();


        //6.Remove both webpages and servers.
        //serverGraphOne.PrintGraph();
        //webGraphOne.PrintGraph();

        serverGraphOne.RemoveServer("B", "A"); //Test removing a server, two existing servers

        serverGraphOne.RemoveServer("A", "Z"); //Test removing a server, source existing

        serverGraphOne.RemoveServer("Z", "A"); //Test removing a server, other server existing

        serverGraphOne.RemoveServer("B", "A"); //Test removing a server, two existing servers

        serverGraphOne.RemoveServer("G", "A"); //Test removing all servers
        serverGraphOne.RemoveServer("D", "A");
        serverGraphOne.RemoveServer("E", "A");
        serverGraphOne.RemoveServer("F", "A");
        serverGraphOne.RemoveServer("A", "A");          //Also test removing a server with both fields having the same server

        //serverGraphOne.PrintGraph();
        //webGraphOne.PrintGraph();

        webGraphOne.RemovePage("005");  //Test removing an existing webpage with no hyperlinks referencing it

        webGraphOne.RemovePage("008");  //Test removing an existing webpage with hyperlinks referencing it

        webGraphOne.RemovePage("008");  //Test removing a non-existing webpage

        //serverGraphOne.PrintGraph();
        //webGraphOne.PrintGraph();

        //7.Determine the articulation points of the remaining Internet.

        //Testing ShortestPath()
        serverGraphOne.AddServer("B", "A");
        serverGraphOne.AddServer("D", "A");
        serverGraphOne.AddServer("C", "D");
        serverGraphOne.AddServer("E", "D");
        serverGraphOne.AddConnection("B", "D");

        //serverGraphOne.PrintGraph();

        int shortestPath = serverGraphOne.ShortestPath("A", "C");           //Test two existing nodes
        //Console.WriteLine("Shortest path from A to C is " + shortestPath);

        shortestPath = serverGraphOne.ShortestPath("A", "A");               //Test a node to itself
       //Console.WriteLine("Shortest path from A to A is " + shortestPath);

        shortestPath = serverGraphOne.ShortestPath("A", "B");               //Test two nodes 1 away from eachother
        //Console.WriteLine("Shortest path from A to B is " + shortestPath);

        shortestPath = serverGraphOne.ShortestPath("E", "A");               //Test two existing nodes 2 away from eachother
        //Console.WriteLine("Shortest path from E to A is " + shortestPath);

        shortestPath = serverGraphOne.ShortestPath("E", "Z");               //Test a non-existing node
        //Console.WriteLine("Shortest path from E to Z is " + shortestPath);


        
        serverGraphOne.PrintGraph();
        var result = serverGraphOne.CriticalServers(); //Test critical servers, 1 critical server

        Console.WriteLine("Critical Servers (0 found if blank):");
        for (int i = 0; i < result.Length; i++) {
            if (result[i] != null) {
                Console.Write(result[i] + ", ");
            }
        }

        //Console.WriteLine("Critical servers: " + result);

        serverGraphOne.AddServer("X", "C");
        serverGraphOne.AddServer("Y", "E");
        serverGraphOne.PrintGraph();
        result = serverGraphOne.CriticalServers(); //Test critical servers, 3 critical servers

        Console.WriteLine("Critical Servers (0 found if blank):");
        for (int i = 0; i < result.Length; i++) {
            if (result[i] != null) {
                Console.Write(result[i] + ", ");
            }
        }


        serverGraphOne.AddConnection("E", "A");
        serverGraphOne.AddConnection("Y", "X");
        serverGraphOne.PrintGraph();
        result = serverGraphOne.CriticalServers(); //Test critical servers, 0 critical servers

        Console.WriteLine("Critical Servers (0 found if blank):");
        for (int i = 0; i < result.Length; i++) {
            if (result[i] != null) {
                Console.Write(result[i] + ", ");
            }
        }



        //8.Calculate the average shortest distance to the hyperlinks of a given webpage.
        webGraphOne.AddPage("010", "B");
        webGraphOne.AddPage("011", "E");
        webGraphOne.AddPage("012", "Y");
        webGraphOne.AddPage("013", "D");

        webGraphOne.AddLink("001", "010");
        webGraphOne.AddLink("010", "012");
        webGraphOne.AddLink("007", "011");
        webGraphOne.AddLink("010", "013");
        webGraphOne.AddLink("013", "012");

        webGraphOne.AddLink("006", "003");
        webGraphOne.AddLink("006", "010");
        webGraphOne.AddLink("006", "011");
        webGraphOne.AddLink("006", "012");
        webGraphOne.AddLink("006", "013");

        serverGraphOne.PrintGraph();    
        webGraphOne.PrintGraph();
        webGraphOne.AvgShortestPaths("003");    //Test a webpage with 1 hyperlink, where both webpages are on the same server

        webGraphOne.AvgShortestPaths("001");    //Test a webpage with 1 hyperlink, both on seperate servers

        webGraphOne.AvgShortestPaths("010");    //Test a webpage with 2 hyperlinks, all different servers

        webGraphOne.AvgShortestPaths("006");    //Test a webpage with 5 hyperlinks

        //In a separate document, illustrate your test cases and compare them with the results of your program.
    }
}