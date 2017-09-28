// This file is part of www.nand2tetris.org
// and the book "The Elements of Computing Systems"
// by Nisan and Schocken, MIT Press.
// File name: projects/04/Fill.asm

// Runs an infinite loop that listens to the keyboard input.
// When a key is pressed (any key), the program blackens the screen,
// i.e. writes "black" in every pixel;
// the screen should remain fully black as long as the key is pressed.
// When no key is pressed, the program clears the screen, i.e. writes
// "white" in every pixel;
// the screen should remain fully clear as long as no key is pressed.

(MAINLOOP)
@KBD                // D = @KBD
D = M
@WHITELOOP          // JMP WHITELOOP, if D == 0
D;JEQ
@BLACKLOOP          // JMP BLACKLOOP, if D > 0
D;JGT

(WHITELOOP)
@i                  // i = 0
M = 0
(WHITEMAINLOOP)
@i
D = M
@8192               // D = i - 8192
D = D - A
@MAINLOOP           // if D == 0; JMP MAINLOOP
D;JEQ
@KBD
D = M
@BLACKLOOP
D;JGT
@i                  // D = i
D = M
@SCREEN             // D = SCREEN + i
D = A + D
A = D               // A = D
M = 0               // M = 0
@i                  // i = i + 1
M = M + 1
@WHITEMAINLOOP      // JMP WHITEMAINLOOP
D;JMP

(BLACKLOOP)
@i
M = 0
(BLACKMAINLOOP)
@i
D = M
@8192
D = D - A
@MAINLOOP
D;JEQ
@KBD
D = M
@WHITELOOP
D;JEQ
@i
D = M
@SCREEN            // address
D = A + D
@R0                // save address
M = D
@1              // D = b1111111111111111
A = -A
D = A
@R0                 // (address) = D
A = M
M = D
@i
M = M + 1
@BLACKMAINLOOP
D;JMP
