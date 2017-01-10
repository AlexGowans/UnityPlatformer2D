/*
 
    Coding Log things to remember

    18th Nov:
    
    When making a button into a bullet button change the collider2d to the correct rotation

    Working on changing buttons from the raycasts which was kind of a pain and just use collider triggers

    I got regular switches working both on the roof and not, next is blaster switches, the whole point of recoding this shit lol


    19th-22nd
    Got buttons working for blasters and standing in one script woo
    Made a prefab of two buttons and a platform for a prebuilt blaster door

    re did all the animations cos i fucked up by editing the sprite sheet

    23rd
    working on the item giver, cant get it to save to an array

    got it to save the array of the item giver as well as the vars for what the player has in the GAME script

    
    3rd Dec
    Grav flip lines work now woo

    Made a branch in git, this is to save an update to test it

    Ok so this is in the branch, gonna be redoing the gravlines into a laser without fucking with the main build, mostly to play
    with git


    4/5th Dec

    Working on beam raycast, calculating correct length etc

    7th Dec

    Think i've finished the upward shooting beam, haven't tested it yet though cos sleep
    Ok it's not working
    But that's tomorrow me's problem

    8th Dec

    Think I found the problem hit.distance wasnt doing what i thought (setting the raylength for the play check to the nearest wall)

    10th Dec

    I got it, re worked it to not use the raycast controller or require a collider, the ray now fires and detects
    but it detects in the 1000's D=

    it's going into the - cos im not doing the nondetection of the beam correctly

    11th Dec

    Seems to work perfect now woo! Had some funny bugs too

    13th Dec

    Made it work with up / down and offset the origin slightly, still don't use both at once, it gets weird
    as they detect separately

    14th Dec

    Trying to get Left/Right beams working

    21st Dec

    Actually did this a few days ago but left right beams are working. Maybe should start taking some vit d to keep my senses up

    28th Dec

    Ended up taking the time around xmas chilling getting onto rendering the beams now

    Jan 9th

    Ok back to it after xmas, gonna get the line renderer going and merge back into master
*/