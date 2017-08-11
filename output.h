#ifndef _OUTPUT_H_
#define _OUTPUT_H_


enum _KEYSIGTYPE {
  CMaj, GMaj, DMaj, AMaj, EMaj, BMaj, FsMaj, CsMaj,
  FMaj, BfMaj, EfMaj, AfMaj, DfMaj, GfMaj, CfMaj,
  Amin, Emin, Bmin, Fsmin, Csmin, Gsmin, Dsmin, Asmin,
  Dmin, Gmin, Cmin, Fmin, Bfmin, Efmin, Afmin
};

enum _NOTETYPE {
  KEYNOTE,
  RESTNOTE
};


struct _LENDATA {
  int size;
  int *data;
  int remain;
  int eval;
};

struct _LENMARK {
  int len;
  unsigned int gate;
};

struct _GATEDATA {
  unsigned int wholecnt;
  unsigned int remaingate;
};


/* void show_events(elist_t **seq); */
char *make_outfilename(char *srcfilename);
int isoutput(char *filename);
void print_events(char *filename, elist_t **seq, unsigned int t_partnum);
void alloclentable(void);
void setlentable(void);
int setlenmk(unsigned int len);
void loop_setlentable(int n, int cnt, int preindex, int size);
void setlendata1(unsigned int index, int size, int *predata, int adddata, int preeval);
void setremainlendata(int n);
void setlendata2(int index, int cnt);
void releaselentable(void);
void read_seqdata(elist_t *seq, unsigned int t_partnum, int num);
void print_partname(void);
void print_tempo(tempo_t *tempo);
void set_keysig(keysig_t *data);
void print_ccpc(ccpc_t *ccpc);
void print_note(note_t *note);
void isoctave(unsigned char key);
void isnewmes(enum _NOTETYPE type, pos_t pos, pos_t endpos, unsigned char key, int tieflg);
struct _GATEDATA calcgate(pos_t pos, pos_t endpos);
struct _GATEDATA addgate(struct _GATEDATA data, unsigned int start, unsigned int end);
void printconvnote(enum _NOTETYPE type, struct _GATEDATA gate, unsigned char key, int tieflg);
void printconvlen(enum _NOTETYPE type, unsigned char key, unsigned int index, int tieflg1, int tieflg2);


#endif /*_OUTPUT_H_*/
