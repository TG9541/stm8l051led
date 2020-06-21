; STM8L051F3P6 "Core" STM8L device dependent routine default code

; Note: for supporting a new board create a new board configuration
;       folder with a "globconfig.inc" and a copy of this file.

; ==============================================

        .ifne   HAS_LED7SEG
;       LED_MPX driver ( -- )
;       Code called from ISR for LED MPX

LED_MPX:
        RET
        .endif

; ==============================================

        .ifne   HAS_OUTPUTS
;       OUT!  ( c -- )
;       Put c to board outputs, storing a copy in OUTPUTS
        .dw     LINK

        LINK =  .
        .db     (4)
        .ascii  "OUT!"
OUTSTOR:
        RET
        .endif

;===============================================================

        .ifne   HAS_KEYS
;       BKEY  ( -- f )     ( TOS STM8: -- A,Z,N )
;       Read board key state as a bitfield
        .dw     LINK

        LINK =  .
        .db     (4)
        .ascii  "BKEY"
BKEY:
        CLR     A
        JP      ASTOR


;       BKEYC  (  -- c )   ( TOS STM8: -- A,Z,N )
;       Read and translate board dependent key bitmap into char

BKEYCHAR:
        JRA     BKEY            ; Dummy: get "no key" and leave it as it is
       .endif

;===============================================================

;       BOARDINIT  ( -- )
;       Init board GPIO (except COM ports)

BOARDINIT:
        ; Board I/O initialization: enable USART TX->PA2, RX->PA3
        MOV     CLK_PCKENR1,#0x25 ; Clock: UART, TIM2, TIM4
        BSET    PA_DDR,#2         ; USART PA2, PA3
        BSET    PA_CR1,#2
        MOV     SYSCFG_RMPCR1,#0x1C

        .ifeq   (PNRX / 4)
        BSET    EXTI_CR1,#1+PNRX*2     ; ext. int. port bit 0..3 falling edge
        .else
        BSET    EXTI_CR2,#1+(PNRX-4)*2 ; ext. int. port bit 4..7 falling edge
        .endif
        RET
