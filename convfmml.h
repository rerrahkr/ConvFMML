#ifndef _CONVFMML_H_
#define _CONVFMML_H_


#include <stdio.h>
#include <stdlib.h>


#define PART_SIZE 64

#define ISEXT(EXT)  (!strcmp(getfileextend(argv[1]), EXT)? 1: 0)
#define FREE(p)     do { free(p); p = NULL; } while (0)
#define EPRINTM()   fprintf(stderr, "[ERROR] 必要分のメモリを確保できませんでした。\n")
#define WAIT()      do { printf("終了するには何かキーを押してください . . ."); getch(); } while (0)
#define ERREXIT()   do { fprintf(stderr, "\n > 変換失敗\n\n"); WAIT(); exit(EXIT_FAILURE); } while (0)


enum _CCPCTYPE {
  VOLUME,
  PAN,
  PC,
  OTHER
};

enum _LISTTYPE {
  RHYTHM = 0,
  MARKER = 1,
  TEMPO  = 2,
  KEYSIG = 3,
  CCPC   = 4,
  NOTE   = 5
};


typedef unsigned char  byte_t;

typedef struct _NOTE    note_t;
typedef struct _CCPC    ccpc_t;
typedef struct _MARKER  marker_t;
typedef struct _TEMPO   tempo_t;
typedef struct _RHYTHM  rhythm_t;
typedef struct _KEYSIG  keysig_t;

typedef struct _TRACK   track_t;
typedef struct _ELIST   elist_t;


/* composition of config */
struct _CONFIG {
  struct _CONFIG_MML {   // mml expression
    int mml_style;
      #define CUSTOM  0
      #define FMP7    1
      #define FMP     2
      #define PMD     3
      #define MXDRV   4
      #define NRTDRV  5
    char *mml_extension;
    int fmp_extension;
    unsigned int timebase_mml;
    int print_timebase;
    int timebase_pmd;
    int newblock_mes;
    int newline_mes;
    int newline_rhythm;
  } mml;
  struct _CONFIG_NOTE {   // note expression
    int octave_newline;
    char *octave_sign;
    int octave_direction;
    int notelength_style;
    int range_notelength;
    char *clockcount_sign;
    int dot_enable;
    int dot_length;
    int measurecut_note;
    char *tie_cmd;
    int resttie_enable;
    int extend_note;
    int tienote_block;
  } note;
  struct _CONFIG_CCPC {   // ccpc expression
    int overlap_ccpc;
    int invalid_ccpc;
    int replace_ccpc;
    int omit_ccpc;
    int volume_enable;
    char *volume_cmd;
    int volume_range;
    int volume_pmd;
    int volume_mxdrv;
    int volume_nrtdrv;
    int vstep_nrtdrv;
    int pan_enable;
    int pan_cmdmode;
    char *pan_midicmd;
    char *pan_lcmd;
    char *pan_ccmd;
    char *pan_rcmd;
    int pan_fmp7;
    int pan_left;
    int pan_right;
    int pc_enable;
    char *pc_cmd;
  } ccpc;
  struct _CONFIG_TEMPO {   // tempo expression
    int tempo_enable;
    char *tempo_cmd;
    int make_tempopart;
  } tempo;
  struct _CONFIG_TRACK {   // track expression
    int print_track;
    int tracksize;
    int number_track[PART_SIZE];
    int print_partname;
    int partsize;
    int fmp7_autoname;
    int fmp_autoname;
    int pmd_autoname;
    int nrtdrv_autoname;
    char *name_part[PART_SIZE];
    char *name_tempopart;
  } track;
};

/* composition of event position */
typedef struct _POSITION {
  unsigned int measure;
  unsigned int miditick;
  unsigned int mmltick;
} pos_t;

/* composition of binary data of chunk */
struct _CHUNK {
  unsigned int size;
  byte_t *event;
};

/* composition of head of event list */
struct _TRACK {
  unsigned short num;
  pos_t eotpos;
  note_t *note_head;
  note_t *note_now;
  ccpc_t *ccpc_head;
  track_t *prev;
  track_t *next;
};

/* composition of note event */
struct _NOTE {
  pos_t pos;
  pos_t endpos;
  unsigned char ch;
  unsigned char key;
  unsigned char velocity;   // 0 is rest
  int tieflg;
  /* [tieflg]
   * in pre, add mescut = 1, ccpccut = 2 to tieflg
   * in next, add mescut = 3, ccpccut = 6 to tieflg
   * 0 = end
   * 3 = pre: none, next: mescut
   * 6 = pre: none, next: ccpccut
   * 1 = pre: mescut, next: end
   * 4 = pre: mescut, next: mescut
   * 7 = pre: mescut, next: ccpccut
   * 2 = pre: ccpccut, next: end
   * 5 = pre: ccpccut, next: mescut
   * 8 = pre: ccpccut, next: ccpccut */
  note_t *prev;
  note_t *next;
};

/* composition of control change event */
struct _CCPC {
  pos_t pos;
  enum _CCPCTYPE type;
  unsigned char ch;
  unsigned char value;
  char *modvalue;
  ccpc_t *prev;
  ccpc_t *next;
};

/* composition of marker event */
struct _MARKER {
  pos_t pos;
  unsigned char *data;
  marker_t *prev;
  marker_t *next;
};

/* composition of tempo change event */
struct _TEMPO {
  pos_t pos;
  unsigned int data;
  tempo_t *prev;
  tempo_t *next;
};

/* composition of rhythm change event */
struct _RHYTHM {
  pos_t pos;
  pos_t endpos;   // signage before modify pos-data
  unsigned char num;
  unsigned char denom_pow2;
  unsigned char beatclock;
  unsigned char hdm_semi;
  unsigned int midimes_tick;  // ticks per 1 measure in midi
  unsigned int mmlmes_tick;
  rhythm_t *prev;
  rhythm_t *next;
};

/* composition of key signature change event */
struct _KEYSIG {
  pos_t pos;
  char sig_num;
  unsigned char minorflg;
  keysig_t *prev;
  keysig_t *next;
};


/* composition of event sequence */
struct _ELIST {
  enum _LISTTYPE type;
  rhythm_t *rhythm;
  marker_t *marker;
  tempo_t *tempo;
  keysig_t *keysig;
  ccpc_t *ccpc;
  note_t *note;
  elist_t *prev;
  elist_t *next;
};


void free_config(void);
int cmppos(pos_t a, pos_t b);
void free_metalists(void);
note_t *make_note_list(note_t *prelist, pos_t pos, note_t **head);
note_t *delnote(note_t *del, note_t **head);
void free_note_list(note_t *list);
void free_ccpc_list(ccpc_t *list);
int istrack(unsigned int num);
track_t *deltrackcell(track_t *del, track_t **head);


extern struct _CONFIG config;

extern unsigned short timebase;
extern double tbratio;
extern pos_t totalpos;

extern marker_t *marker_head;
extern tempo_t *tempo_head;
extern rhythm_t *rhythm_head;
extern keysig_t *keysig_head;

extern elist_t *t_part;


#endif /*_CONVFMML_H_*/
