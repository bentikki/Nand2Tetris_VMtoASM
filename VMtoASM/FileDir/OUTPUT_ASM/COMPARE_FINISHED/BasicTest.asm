@256
D=A
@SP
M=D

// 0: push constant 10

@10
D=A
@SP
A=M
M=D
@SP
M=M+1

// 1: pop local 0

@LCL
D=M
@0
D=D+A
@R13
M=D
@SP
AM=M-1
D=M
@R13
A=M
M=D

// 2: push constant 21

@21
D=A
@SP
A=M
M=D
@SP
M=M+1

// 3: push constant 22

@22
D=A
@SP
A=M
M=D
@SP
M=M+1

// 4: pop argument 2

@ARG
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

// 5: pop argument 1

@ARG
D=M
@1
D=D+A
@R13
M=D
@SP
AM=M-1
D=M
@R13
A=M
M=D

// 6: push constant 36

@36
D=A
@SP
A=M
M=D
@SP
M=M+1

// 7: pop this 6

@THIS
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

// 8: push constant 42

@42
D=A
@SP
A=M
M=D
@SP
M=M+1

// 9: push constant 45

@45
D=A
@SP
A=M
M=D
@SP
M=M+1

// 10: pop that 5

@THAT
D=M
@5
D=D+A
@R13
M=D
@SP
AM=M-1
D=M
@R13
A=M
M=D

// 11: pop that 2

@THAT
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

// 12: push constant 510

@510
D=A
@SP
A=M
M=D
@SP
M=M+1

// 13: pop temp 6

@R5
D=M
@11
D=D+A
@R13
M=D
@SP
AM=M-1
D=M
@R13
A=M
M=D

// 14: push local 0

@LCL
D=M
@0
A=D+A
D=M
@SP
A=M
M=D
@SP
M=M+1

// 15: push that 5

@THAT
D=M
@5
A=D+A
D=M
@SP
A=M
M=D
@SP
M=M+1

// 16: add

@SP
AM=M-1
D=M
A=A-1
M=M+D

// 17: push argument 1

@ARG
D=M
@1
A=D+A
D=M
@SP
A=M
M=D
@SP
M=M+1

// 18: sub

@SP
AM=M-1
D=M
A=A-1
M=M-D
D=0

// 19: push this 6

@THIS
D=M
@6
A=D+A
D=M
@SP
A=M
M=D
@SP
M=M+1

// 20: push this 6

@THIS
D=M
@6
A=D+A
D=M
@SP
A=M
M=D
@SP
M=M+1

// 21: add

@SP
AM=M-1
D=M
A=A-1
M=M+D

// 22: sub

@SP
AM=M-1
D=M
A=A-1
M=M-D
D=0

// 23: push temp 6

@R5
D=M
@11
A=D+A
D=M
@SP
A=M
M=D
@SP
M=M+1

// 24: add

@SP
AM=M-1
D=M
A=A-1
M=M+D
