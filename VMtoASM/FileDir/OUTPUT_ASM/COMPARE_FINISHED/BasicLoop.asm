@256
D=A
@SP
M=D

// 0: push constant 0

@0
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

// 2: label loop_start

(loop_start)

// 3: push argument 0

@ARG
D=M
@0
A=D+A
D=M
@SP
A=M
M=D
@SP
M=M+1

// 4: push local 0

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

// 5: add

@SP
AM=M-1
D=M
A=A-1
M=M+D

// 6: pop local 0

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

// 7: push argument 0

@ARG
D=M
@0
A=D+A
D=M
@SP
A=M
M=D
@SP
M=M+1

// 8: push constant 1

@1
D=A
@SP
A=M
M=D
@SP
M=M+1

// 9: sub

@SP
AM=M-1
D=M
A=A-1
M=M-D
D=0

// 10: pop argument 0

@ARG
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

// 11: push argument 0

@ARG
D=M
@0
A=D+A
D=M
@SP
A=M
M=D
@SP
M=M+1

// 12: if-goto loop_start

@SP
AM=M-1
D=M
A=A-1
@loop_start
D;JNE

// 13: push local 0

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
