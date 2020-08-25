\ test for Andrew Clapp's tube amplifier data transmission
#require CRC16

NVM
#require TICKCNT
#require 'IDLE

  VARIABLE SSTATE
  VARIABLE SBUF 2 ALLOT
  VARIABLE START
  VARIABLE STOP

  : sendtask ( -- )
    SSTATE @ 1+ 3 AND DUP SSTATE ! ( n )
    DUP 3 = IF
      DROP TIM SBUF !
      -1 SBUF C@ CRC16 SBUF 1+ C@ CRC16 SBUF 2+ C!
    ELSE
      ( n ) SBUF + C@ TXP!
    THEN
  ;

  : r START @ TICKCNT ! ;
   
  : t STOP @ TIM < IF r THEN ;
  
  : init 
    $FFFC START !
    $0003 STOP  !
    r
    [ ' sendtask ] LITERAL BG !
    [ ' t ] LITERAL 'IDLE !
  ;

  ' init 'BOOT !
RAM
