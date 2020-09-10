Lab 4 README File

Controls
--------

Thumbstick [Touch]      creates ray, highlights object if hits it
Thumbstick [Click]      selects the object so it stays highlighted
                        if ray not hitting object, deselected object
Trigger                 can interact with buttons, joystick, pull rod

Notes
-----

- did not get haptics working
- had code:
public SteamVR_Action_Vibration hapticAction;
hapticAction.Execute(0, 1, 150, 75, SteamVR_Input_Sources.LeftHand);
- no errors but no response from controller