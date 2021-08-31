@256
D=A
@SP
M=D

// 0: push constant 17

@17
D=A
@SP
A=M
M=D
@SP
M=M+1

// 1: push constant 17

@17
D=A
@SP
A=M
M=D
@SP
M=M+1

// 2: eq

@SP
AM=M-1
D=M
A=A-1
D=M-D
@FALSE0
D;JNE
@SP
A=M-1
M=-1
@CONTINUE1
0;JMP
(FALSE0)
@SP
A=M-1
M=0
(CONTINUE1)

// 3: push constant 17

@17
D=A
@SP
A=M
M=D
@SP
M=M+1

// 4: push constant 16

@16
D=A
@SP
A=M
M=D
@SP
M=M+1

// 5: eq

@SP
AM=M-1
D=M
A=A-1
D=M-D
@FALSE2
D;JNE
@SP
A=M-1
M=-1
@CONTINUE3
0;JMP
(FALSE2)
@SP
A=M-1
M=0
(CONTINUE3)

// 6: push constant 16

@16
D=A
@SP
A=M
M=D
@SP
M=M+1

// 7: push constant 17

@17
D=A
@SP
A=M
M=D
@SP
M=M+1

// 8: eq

@SP
AM=M-1
D=M
A=A-1
D=M-D
@FALSE4
D;JNE
@SP
A=M-1
M=-1
@CONTINUE5
0;JMP
(FALSE4)
@SP
A=M-1
M=0
(CONTINUE5)

// 9: push constant 892

@892
D=A
@SP
A=M
M=D
@SP
M=M+1

// 10: push constant 891

@891
D=A
@SP
A=M
M=D
@SP
M=M+1

// 11: lt

@SP
AM=M-1
D=M
A=A-1
D=M-D
@FALSE6
D;JGE
@SP
A=M-1
M=-1
@CONTINUE7
0;JMP
(FALSE6)
@SP
A=M-1
M=0
(CONTINUE7)

// 12: push constant 891

@891
D=A
@SP
A=M
M=D
@SP
M=M+1

// 13: push constant 892

@892
D=A
@SP
A=M
M=D
@SP
M=M+1

// 14: lt

@SP
AM=M-1
D=M
A=A-1
D=M-D
@FALSE8
D;JGE
@SP
A=M-1
M=-1
@CONTINUE9
0;JMP
(FALSE8)
@SP
A=M-1
M=0
(CONTINUE9)

// 15: push constant 891

@891
D=A
@SP
A=M
M=D
@SP
M=M+1

// 16: push constant 891

@891
D=A
@SP
A=M
M=D
@SP
M=M+1

// 17: lt

@SP
AM=M-1
D=M
A=A-1
D=M-D
@FALSE10
D;JGE
@SP
A=M-1
M=-1
@CONTINUE11
0;JMP
(FALSE10)
@SP
A=M-1
M=0
(CONTINUE11)

// 18: push constant 32767

@32767
D=A
@SP
A=M
M=D
@SP
M=M+1

// 19: push constant 32766

@32766
D=A
@SP
A=M
M=D
@SP
M=M+1

// 20: gt

@SP
AM=M-1
D=M
A=A-1
D=M-D
@FALSE12
D;JLE
@SP
A=M-1
M=-1
@CONTINUE13
0;JMP
(FALSE12)
@SP
A=M-1
M=0
(CONTINUE13)

// 21: push constant 32766

@32766
D=A
@SP
A=M
M=D
@SP
M=M+1

// 22: push constant 32767

@32767
D=A
@SP
A=M
M=D
@SP
M=M+1

// 23: gt

@SP
AM=M-1
D=M
A=A-1
D=M-D
@FALSE14
D;JLE
@SP
A=M-1
M=-1
@CONTINUE15
0;JMP
(FALSE14)
@SP
A=M-1
M=0
(CONTINUE15)

// 24: push constant 32766

@32766
D=A
@SP
A=M
M=D
@SP
M=M+1

// 25: push constant 32766

@32766
D=A
@SP
A=M
M=D
@SP
M=M+1

// 26: gt

@SP
AM=M-1
D=M
A=A-1
D=M-D
@FALSE16
D;JLE
@SP
A=M-1
M=-1
@CONTINUE17
0;JMP
(FALSE16)
@SP
A=M-1
M=0
(CONTINUE17)

// 27: push constant 57

@57
D=A
@SP
A=M
M=D
@SP
M=M+1

// 28: push constant 31

@31
D=A
@SP
A=M
M=D
@SP
M=M+1

// 29: push constant 53

@53
D=A
@SP
A=M
M=D
@SP
M=M+1

// 30: add

@SP
AM=M-1
D=M
A=A-1
M=M+D

// 31: push constant 112

@112
D=A
@SP
A=M
M=D
@SP
M=M+1

// 32: sub

@SP
AM=M-1
D=M
A=A-1
M=M-D
D=0

// 33: neg

@SP
A=M-1
M=D-M

// 34: and

@SP
AM=M-1
D=M
A=A-1
M=M&D

// 35: push constant 82

@82
D=A
@SP
A=M
M=D
@SP
M=M+1

// 36: or

@SP
AM=M-1
D=M
A=A-1
M=M|D

// 37: not

@SP
A=M-1
M=!M
