# Author: Martin Matusiak <numerodix@gmail.com>
# Licensed under the GNU Public License, version 3.

import clr
import System

clr.AddReference('gdk-sharp'); import Gdk


GUITHREAD = None

def gdk_enter():
    assert(GUITHREAD != None)
    if System.Threading.Thread.CurrentThread != GUITHREAD:
        Gdk.Threads.Enter()

def gdk_leave():
    assert(GUITHREAD != None)
    if System.Threading.Thread.CurrentThread != GUITHREAD:
        Gdk.Threads.Leave()
