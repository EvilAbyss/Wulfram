using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MBS;

public class CMLTest : MonoBehaviour {

	CML database;

	void Start()
	{
		database = new CML();

		//lets create our first node...
		database.AddNode("Library");

		//for our second node, we are going to add a field to the node...
		database.AddNode("Section", "Cat=History");

		//for our third node, we are going to add a bunch of fields to our node.
		database.AddNode("Book", "name=myBad Studios, a History; price=199.99; author=DrDude");

		//and then we are going to add a bunch of strings to an unordered list...
		database.Last.AddToData("There once was a company called myBad Studios");
		database.Last.AddToData("They rocked!!!");

		// lets just store the Id of this node for later...
		int id = database.Last.ID;

		//just for fun, let's add subnodes to this Book node. Let's save checkout history...
		database.AddNode("checkout", "date=2012/12/13; person=id1234");

		//thanks to default values, you don't have to specify numbers with a value of 0 or
		//booleans with a value of false in order to use them later on... 
		//above I didn't specify the staff value. Here I do.
		//In both cases you can still use staff as a boolean value without getting an error.
		database.AddNode("checkout", "date=2013/01/12; person=id112; staff=true");

		//and now let's make a new book node...
		database.AddNode("Book", "name=CML for Dummies; price=9.95; author=DrDude");
		database.Last.AddToData("CML is so easy I don't know what to write in this book...");
		database.Last.AddToData("The end");

		//oh wait, I never ended off the first book.... Fortunately, I have the node index so...
		//let me add some more text to that other node directly. No navigation needed.
		database[id].AddToData("To be continued...");

		//so, let's see how many sections I have in this database...
		List<int> cats = database.AllNodesOfTypei("Section");
		Debug.Log ("There are " + cats.Count + " sections in this library");

		//so, let's see how many books I have in each section. I will be accessing the
		//sections directly via index, entirely skipping any node traversal requirements
		foreach(int i in cats)
		{
			//fetch all "Book" nodes as children of "Section" nodes
			List<cmlData> books = database.Children(i);

			//and let's find the most expensive one of them all...
			float price = 0f;
			int index = 0;

			//I fetch the price as a float and compare it to the stored price...
			if (null != books)
			foreach (cmlData book in books)
			{
				if (book.Float("price") > price)
				{
					price = book.Float("price");
					index = book.ID;
				}
			}

			//now that I've looked at all the Books in the History Section,
			//I have the ID of the node of the most expensive book so I can
			//again access the Book node directly and extract any string or float or whatever is there...
			if (index > 0)
			{
				Debug.Log("The most expensive book was: " + database[index].String("name") + " with a value of " + database[index].Float("price"));
				Debug.Log ("The contents of the book is:");
				foreach (string s in database[index].data)
					Debug.Log (s);

				//now, let's check out the checkout history.
				//let's start by fetching all "checkout" nodes. since they are all stored after the
				//relevant Book node, I can fetch them as children of the book so...
				bool staff = false;
				List<cmlData> checkout_history = database.Children(index);

				if (null == checkout_history)
				{
					Debug.Log("this book was never checked out!");
				} else {
					//let's print off the checkout dates for this book...
					foreach(cmlData history in checkout_history)
					{
						if (history.Bool("staff")) staff = true;
						Debug.Log ("This book was checked out on " + history.String("date"));
					}
					if (staff)
						Debug.Log ("This book has been checked out by staff");
					else
						Debug.Log ("This book has not passed quality control");
				}
			}
		}

		//and just for good measure, let's print out the entire file so you can see what CML looks like
		Debug.Log( database.ToString() );
	}
	
}