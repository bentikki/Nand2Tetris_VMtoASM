@256
D=A
@SP
M=D

// 0: function sys.init 0

@sys.init_RETURN_LABEL0
D=A
@SP
A=M
M=D
@SP
M=M+1
@LCL
D=M
@SP
A=M
M=D
@SP
M=M+1
@ARG
D=M
@SP
A=M
M=D
@SP
M=M+1
@THIS
D=M
@SP
A=M
M=D
@SP
M=M+1
@THAT
D=M
@SP
A=M
M=D
@SP
M=M+1
@SP
D=M
@5
D=D-A
@0
D=D-A
@ARG
M=D
@SP
D=M
@LCL
M=D
@sys.init
0;JMP
(sys.init_RETURN_LABEL0)
(sys.init)

// 1: push constant 4

@4
D=A
@SP
A=M
M=D
@SP
M=M+1

// 2: call main.fibonacci 1

@main.fibonacci_RETURN_LABEL1
D=A
@SP
A=M
M=D
@SP
M=M+1
@LCL
D=M
@SP
A=M
M=D
@SP
M=M+1
@ARG
D=M
@SP
A=M
M=D
@SP
M=M+1
@THIS
D=M
@SP
A=M
M=D
@SP
M=M+1
@THAT
D=M
@SP
A=M
M=D
@SP
M=M+1
@SP
D=M
@5
D=D-A
@1
D=D-A
@ARG
M=D
@SP
D=M
@LCL
M=D
@main.fibonacci
0;JMP
(main.fibonacci_RETURN_LABEL1)

// 3: label while

(while)

// 4: goto while

@while
0;JMP

// 5: function main.fibonacci 0

(main.fibonacci)

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

// 8: lt

@SP
AM=M-1
D=M
A=A-1
D=M-D
@FALSE2
D;JGE
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

// 9: if-goto if_true

@SP
AM=M-1
D=M
A=A-1
@if_true
D;JNE

// 10: goto if_false

@if_false
0;JMP

// 11: label if_true

(if_true)

// 12: push argument 0

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

// 13: return

@LCL
D=M
@FRAME
M=D
@5
A=D-A
D=M
@RET
M=D
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
@ARG
D=M
@SP
M=D+1
@FRAME
D=M-1
AM=D
D=M
@THAT
M=D
@FRAME
D=M-1
AM=D
D=M
@THIS
M=D
@FRAME
D=M-1
AM=D
D=M
@ARG
M=D
@FRAME
D=M-1
AM=D
D=M
@LCL
M=D
@RET
A=M
0;JMP

// 14: label if_false

(if_false)

// 15: push argument 0

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

// 16: push constant 2

@2
D=A
@SP
A=M
M=D
@SP
M=M+1

// 17: sub

@SP
AM=M-1
D=M
A=A-1
M=M-D
D=0

// 18: call main.fibonacci 1

@main.fibonacci_RETURN_LABEL4
D=A
@SP
A=M
M=D
@SP
M=M+1
@LCL
D=M
@SP
A=M
M=D
@SP
M=M+1
@ARG
D=M
@SP
A=M
M=D
@SP
M=M+1
@THIS
D=M
@SP
A=M
M=D
@SP
M=M+1
@THAT
D=M
@SP
A=M
M=D
@SP
M=M+1
@SP
D=M
@5
D=D-A
@1
D=D-A
@ARG
M=D
@SP
D=M
@LCL
M=D
@main.fibonacci
0;JMP
(main.fibonacci_RETURN_LABEL4)

// 19: push argument 0

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

// 20: push constant 1

@1
D=A
@SP
A=M
M=D
@SP
M=M+1

// 21: sub

@SP
AM=M-1
D=M
A=A-1
M=M-D
D=0

// 22: call main.fibonacci 1

@main.fibonacci_RETURN_LABEL5
D=A
@SP
A=M
M=D
@SP
M=M+1
@LCL
D=M
@SP
A=M
M=D
@SP
M=M+1
@ARG
D=M
@SP
A=M
M=D
@SP
M=M+1
@THIS
D=M
@SP
A=M
M=D
@SP
M=M+1
@THAT
D=M
@SP
A=M
M=D
@SP
M=M+1
@SP
D=M
@5
D=D-A
@1
D=D-A
@ARG
M=D
@SP
D=M
@LCL
M=D
@main.fibonacci
0;JMP
(main.fibonacci_RETURN_LABEL5)

// 23: add

@SP
AM=M-1
D=M
A=A-1
M=M+D

// 24: return

@LCL
D=M
@FRAME
M=D
@5
A=D-A
D=M
@RET
M=D
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
@ARG
D=M
@SP
M=D+1
@FRAME
D=M-1
AM=D
D=M
@THAT
M=D
@FRAME
D=M-1
AM=D
D=M
@THIS
M=D
@FRAME
D=M-1
AM=D
D=M
@ARG
M=D
@FRAME
D=M-1
AM=D
D=M
@LCL
M=D
@RET
A=M
0;JMP
