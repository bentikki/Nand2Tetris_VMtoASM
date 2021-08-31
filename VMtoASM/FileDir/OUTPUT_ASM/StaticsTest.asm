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

// 1: push constant 6

@6
D=A
@SP
A=M
M=D
@SP
M=M+1

// 2: push constant 8

@8
D=A
@SP
A=M
M=D
@SP
M=M+1

// 3: call class1.set 2

@class1.set_RETURN_LABEL1
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
@2
D=D-A
@ARG
M=D
@SP
D=M
@LCL
M=D
@class1.set
0;JMP
(class1.set_RETURN_LABEL1)

// 4: pop temp 0

@R5
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

// 5: push constant 23

@23
D=A
@SP
A=M
M=D
@SP
M=M+1

// 6: push constant 15

@15
D=A
@SP
A=M
M=D
@SP
M=M+1

// 7: call class2.set 2

@class2.set_RETURN_LABEL2
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
@2
D=D-A
@ARG
M=D
@SP
D=M
@LCL
M=D
@class2.set
0;JMP
(class2.set_RETURN_LABEL2)

// 8: pop temp 0

@R5
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

// 9: call class1.get 0

@class1.get_RETURN_LABEL3
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
@class1.get
0;JMP
(class1.get_RETURN_LABEL3)

// 10: call class2.get 0

@class2.get_RETURN_LABEL4
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
@class2.get
0;JMP
(class2.get_RETURN_LABEL4)

// 11: label while

(while)

// 12: goto while

@while
0;JMP

// 13: function class1.set 0

(class1.set)

// 14: push argument 0

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

// 15: pop static 0

@class1.static0
D=A
@R13
M=D
@SP
AM=M-1
D=M
@R13
A=M
M=D

// 16: push argument 1

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

// 17: pop static 1

@class1.static1
D=A
@R13
M=D
@SP
AM=M-1
D=M
@R13
A=M
M=D

// 18: push constant 0

@0
D=A
@SP
A=M
M=D
@SP
M=M+1

// 19: return

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

// 20: function class1.get 0

(class1.get)

// 21: push static 0

@class1.static0
D=M
@SP
A=M
M=D
@SP
M=M+1

// 22: push static 1

@class1.static1
D=M
@SP
A=M
M=D
@SP
M=M+1

// 23: sub

@SP
AM=M-1
D=M
A=A-1
M=M-D
D=0

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

// 25: function class2.set 0

(class2.set)

// 26: push argument 0

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

// 27: pop static 0

@class2.static0
D=A
@R13
M=D
@SP
AM=M-1
D=M
@R13
A=M
M=D

// 28: push argument 1

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

// 29: pop static 1

@class2.static1
D=A
@R13
M=D
@SP
AM=M-1
D=M
@R13
A=M
M=D

// 30: push constant 0

@0
D=A
@SP
A=M
M=D
@SP
M=M+1

// 31: return

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

// 32: function class2.get 0

(class2.get)

// 33: push static 0

@class2.static0
D=M
@SP
A=M
M=D
@SP
M=M+1

// 34: push static 1

@class2.static1
D=M
@SP
A=M
M=D
@SP
M=M+1

// 35: sub

@SP
AM=M-1
D=M
A=A-1
M=M-D
D=0

// 36: return

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
