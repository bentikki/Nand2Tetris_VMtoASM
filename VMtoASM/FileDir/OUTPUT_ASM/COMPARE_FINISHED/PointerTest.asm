@256
D=A
@SP
M=D

// 0: push constant 3030

@3030
D=A
@SP
A=M
M=D
@SP
M=M+1

// 1: pop pointer 0

@THIS
D=A
@R13
M=D
@SP
AM=M-1
D=M
@R13
A=M
M=D

// 2: push constant 3040

@3040
D=A
@SP
A=M
M=D
@SP
M=M+1

// 3: pop pointer 1

@THAT
D=A
@R13
M=D
@SP
AM=M-1
D=M
@R13
A=M
M=D

// 4: push constant 32

@32
D=A
@SP
A=M
M=D
@SP
M=M+1

// 5: pop this 2

@THIS
D=M
@2
D=D+A
@R13
M=D
@SP
AM=M-1
D=M
@R13
A=M
M=D

// 6: push constant 46

@46
D=A
@SP
A=M
M=D
@SP
M=M+1

// 7: pop that 6

@THAT
D=M
@6
D=D+A
@R13
M=D
@SP
AM=M-1
D=M
@R13
A=M
M=D

// 8: push pointer 0

@THIS
D=M
@SP
A=M
M=D
@SP
M=M+1

// 9: push pointer 1

@THAT
D=M
@SP
A=M
M=D
@SP
M=M+1

// 10: add

@SP
AM=M-1
D=M
A=A-1
M=M+D

// 11: push this 2

@THIS
D=M
@2
A=D+A
D=M
@SP
A=M
M=D
@SP
M=M+1

// 12: sub

@SP
AM=M-1
D=M
A=A-1
M=M-D
D=0

// 13: push that 6

@THAT
D=M
@6
A=D+A
D=M
@SP
A=M
M=D
@SP
M=M+1

// 14: add

@SP
AM=M-1
D=M
A=A-1
M=M+D
