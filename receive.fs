#require CRC16
#require PINDEBUG
#require ]C!

8500  CONSTANT CLOW
6500  CONSTANT CHIGH


\res MCU: STM8L
NVM
\res export CLK_HSICALR CLK_HSITRIMR CLK_HSIUNLCKR
  #require 'IDLE
  #require TSTAMP
  VARIABLE RSTATE
  VARIABLE RBUF 2 ALLOT
  VARIABLE DAT
  VARIABLE VAL
  VARIABLE NEWVAL
  VARIABLE TLOW
  VARIABLE THIGH

  : trim ( c -- ) 
    CLK_HSITRIMR 
    [ $AC CLK_HSIUNLCKR ]C! 
    [ $35 CLK_HSIUNLCKR ]C! 
    C!
  ;

  : fast ( -- )
    CLK_HSICALR C@ 7 + trim
  ;

  : normal ( -- )
    CLK_HSICALR C@ trim
  ;

  : slow ( -- )
    CLK_HSICALR C@ 7 - trim
  ;

  : PLL ( -- )
    TLOW @ TSTAMP @ < IF
      P2H
      slow
    ELSE THIGH @ TSTAMP @ < IF
        P2L
        P3L
        normal 
      ELSE
        P3H
        fast
      THEN
    THEN
  ;

  : rectask ( -- )
    P1H
    ?RXP IF
      PLL
      ( c ) RSTATE @ 3 AND SWAP
      ( n c ) OVER RBUF + ( n c a ) C!
      ( n ) 1+ RSTATE !
    ELSE
      0 RSTATE !
      -1 RBUF    C@ CRC16 ( n )
         RBUF 1+ C@ CRC16 ( n )
      255 AND RBUF 2+ C@ = IF RBUF @ ELSE -1 THEN
      VAL ! RBUF @ DAT !
      -1 NEWVAL !
    THEN
    P1L
  ;

  : v NEWVAL @ IF 0 NEWVAL ! HEX val ? dat ? RBUF 2+ C@ .  DECIMAL cr THEN ;

  : s [ ' v ] LITERAL 'IDLE ! ;

  : r 0 'IDLE ! ;

  : init
    CLOW TLOW !
    CHIGH THIGH !
    PINDEBUG
    [ ' rectask ] LITERAL BG !
    s
  ;

  ' init 'BOOT !

RAM


\\ 

#require CRC16
#require PINDEBUG

\res MCU: STM8L051
\res export TIM2_ARRH

#require TSTAMP

9750  CONSTANT BGFAST
9950  CONSTANT BGNORMAL
10150 CONSTANT BGSLOW

8500  CONSTANT CLOW
6500  CONSTANT CHIGH

NVM
  #require 'IDLE
  VARIABLE RSTATE
  VARIABLE RBUF 2 ALLOT
  VARIABLE DAT
  VARIABLE VAL
  VARIABLE TLOW
  VARIABLE THIGH
  VARIABLE NEWVAL

  : sARR ( n  -- ) TIM2_ARRH 2C! ; 

  : PLL ( -- )
    TSTAMP @ TLOW OVER < IF
      BGFAST sARR
    ELSE THIGH < IF
        BGNORMAL sARR
      ELSE
        BGSLOW sARR
      THEN
    THEN
  ;

  : rectask ( -- )
    P1H
    ?RXP IF
      PLL
      ( c ) RSTATE @ 3 AND SWAP
      ( n c ) OVER RBUF + ( n c a ) C!
      ( n ) 1+ RSTATE !
    ELSE
      0 RSTATE !
      -1 RBUF    C@ CRC16 ( n )
         RBUF 1+ C@ CRC16 ( n )
      255 AND RBUF 2+ C@ = IF RBUF @ ELSE -1 THEN
      VAL ! RBUF @ DAT !
      -1 NEWVAL !
    THEN
    P1L
  ;

  : v NEWVAL @ IF 0 NEWVAL ! HEX val ? dat ? RBUF 2+ C@ .  DECIMAL cr THEN ;

  : s [ ' v ] LITERAL 'IDLE ! ;

  : r 0 'IDLE ! ;

  : init
    PINDEBUG

    [ ' rectask ] LITERAL BG !
    s
  ;

  ' init 'BOOT !

RAM
