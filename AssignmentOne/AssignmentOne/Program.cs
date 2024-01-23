using System.Data.Common;
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

    // 4 marks
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
    
    // 4 marks DONE
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
    private ServerGraph serverGraph;
    //...

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

    // 8 marks
    // Remove the webpage with the given name, including the hyperlinks
    // from and to the webpage
    // Return true if successful; otherwise return false
    public bool RemovePage(string name) {
        return false;
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

    // 6 marks
    // Return the average length of the shortest paths from the webpage with
    // given name to each of its hyperlinks
    // Hint: Use the method ShortestPath in the class ServerGraph
    public float AvgShortestPaths(string name) { 
        return 0f;
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


class Program
{
    static void Main(string[] args)
    {
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
        serverGraphOne.PrintGraph();
        webGraphOne.PrintGraph();

        serverGraphOne.RemoveServer("B", "A"); //Test removing a server, two existing servers

        serverGraphOne.RemoveServer("A", "Z"); //Test removing a server, source existing

        serverGraphOne.RemoveServer("Z", "A"); //Test removing a server, other server existing

        serverGraphOne.RemoveServer("B", "A"); //Test removing a server, two existing servers

        serverGraphOne.RemoveServer("G", "A"); //Test removing all servers
        serverGraphOne.RemoveServer("D", "A");
        serverGraphOne.RemoveServer("E", "A");
        serverGraphOne.RemoveServer("F", "A");
        serverGraphOne.RemoveServer("A", "A");          //Also test removing a server with both fields having the same server

        serverGraphOne.PrintGraph();
        webGraphOne.PrintGraph();

        //7.Determine the articulation points of the remaining Internet.
        //8.Calculate the average shortest distance to the hyperlinks of a given webpage.


        //In a separate document, illustrate your test cases and compare them with the results of your program.
    }
}