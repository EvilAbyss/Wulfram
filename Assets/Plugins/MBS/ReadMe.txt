myBad Studios Core
==================

This kit contains the classes I tend to use in every project I do.

They form the core of all my work and is thus made available as a separate download to facilitate
an easier upgrade process as they are improved upon. In time other parts of code may join the
ranks of the core group and will be added to this package when they do.

The core components include:
1. mbsStateMachine
------------------
My custom designed state machine. It allows me to create a layer of abstraction to replace long
and unsightly if-else code blocks. Super simple to use, no matter what project I'm doing I always
find a need for this...

2. mbsSlider
------------
In video games, animations is always good. Static panels and stuff that just appear and disappear
in the blink of an eye just look amateurish. This is a low level class that you simply tell where
the final panel is to appear, what direction to slide in from and slide out to and then call Update
to do it. This class will then provide you with a Rect that you use to draw all your content to and
also throws out events when reaching the opened or closed state in case you want it.
Also contains code to fade the content in and out as the panel slides.

3. CML
------
CML is a custom data storage and retrieval system. To best describe it, think of it is as XML, only easier.
CML also contains a built in save and load system that utilises PlayerPrefs, Resources folders and file paths.

And finally, CML also allows you to generate variables dynamically at runtime and use them as typed variables.
CML is even smart enough to provide default values for when you use a variable that doesn't exist thereby
preventing runtime errors and allowing you to save less data to disc.

4. GUIX
-------
I like forcing the display to a set size and hardcoding my GUI elements to that screen size. This means
I can hard code my displays and it will still fit on any device at any resolution and asspect ratio.
It is not ideal in all cases but in most cases this is exatly what I need so I am including it for you also.

5. PleaseWait
-------------
With a sigle line of code, display a spinning image in the center of the screen as a progress indicator.
You can add the PleaseWait component to a prefab and assign an image of your choice or you can just start
using it and it will create a persistent game object automatically. If it is created automatically then it
will require an image called Spinner inside a Resources foler.

6. Encoder
----------
Encode a string to MD5 or base64 and decode base64 back to string using a single line of code for each
operation, respectively

7. StatusMessage
----------------
Show error messages on screen. Messages can stack and will automatically disappear in a few seconds

NOTE: ALL CLASSES ARE INSIDE THE MBS NAMESPACE.
MAKE SURE TO ADD "using MBS;" AT THE TOP

Included in the kit is a folder with some example scripts that demonstrate the use of each component.
Simply open up the source and follow along with the comments. To see the code in action, simply drop onto
an empty game object and hit play.