@256
D=A
@SP
M=D

// 0: push argument 1

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

// 1: pop pointer 1

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

// 2: push constant 0

@0
D=A
@SP
A=M
M=D
@SP
M=M+1

// 3: pop that 0

@THAT
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

// 4: push constant 1

@1
D=A
@SP
A=M
M=D
@SP
M=M+1

// 5: pop that 1

@THAT
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

// 6: push argument 0

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

// 7: push constant 2

@2
D=A
@SP
A=M
M=D
@SP
M=M+1

// 8: sub

@SP
AM=M-1
D=M
A=A-1
M=M-D
D=0

// 9: pop argument 0

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

// 10: label main_loop_start

(main_loop_start)

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

// 12: if-goto compute_element

@SP
AM=M-1
D=M
A=A-1
@compute_element
D;JNE

// 13: goto end_program

@end_program
0;JMP

// 14: label compute_element

(compute_element)

// 15: push that 0

@THAT
D=M
@0
A=D+A
D=M
@SP
A=M
M=D
@SP
M=M+1

// 16: push that 1

@THAT
D=M
@1
A=D+A
D=M
@SP
A=M
M=D
@SP
M=M+1

// 17: add

@SP
AM=M-1
D=M
A=A-1
M=M+D

// 18: pop that 2

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

// 19: push pointer 1

@THAT
D=M
@SP
A=M
M=D
@SP
M=M+1

// 20: push constant 1

@1
D=A
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

// 22: pop pointer 1

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

// 23: push argument 0

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

// 24: push constant 1

@1
D=A
@SP
A=M
M=D
@SP
M=M+1

// 25: sub

@SP
AM=M-1
D=M
A=A-1
M=M-D
D=0

// 26: pop argument 0

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

// 27: goto main_loop_start

@main_loop_start
0;JMP

// 28: label end_program

(end_program)
