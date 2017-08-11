#include <string.h>
#include "convfmml.h"
#include "output.h"


static FILE *fp;
static struct _LENDATA *lentable;
static struct _LENMARK *lenmk;

static rhythm_t *rhythm_now;
static int partindex=0;
static int lastflg;
static int tempopartflg;
static int mescnt, premescnt;
static int mesflg;
/* [mesflg]
 * 0 = no-action
 * 1 = new-line from full-tick of measure,
 * 2 = space from new measure, minus=leave action */
static int noteeventflg;
static int octaveflg;
static int preoctave;
static enum _KEYSIGTYPE keysigtype;
static char *keytable[30][13]={
  {"c",  "c+", "d", "d+", "e",  "f",  "f+", "g", "g+", "a", "a+", "b",  "r"},   // CMaj
  {"c",  "c+", "d", "d+", "e",  "f",  "f+", "g", "g+", "a", "a+", "b",  "r"},   // GMaj
  {"c",  "c+", "d", "d+", "e",  "f",  "f+", "g", "g+", "a", "a+", "b",  "r"},   // DMaj
  {"c",  "c+", "d", "d+", "e",  "f",  "f+", "g", "g+", "a", "a+", "b",  "r"},   // AMaj
  {"c",  "c+", "d", "d+", "e",  "f",  "f+", "g", "g+", "a", "a+", "b",  "r"},   // EMaj
  {"c",  "c+", "d", "d+", "e",  "f",  "f+", "g", "g+", "a", "a+", "b",  "r"},   // BMaj
  {"c",  "c+", "d", "d+", "e",  "e+", "f+", "g", "g+", "a", "a+", "b",  "r"},   // FsMaj
  {"b+", "c+", "d", "d+", "e",  "e+", "f+", "g", "g+", "a", "a+", "b",  "r"},   // CsMaj
  {"c",  "c+", "d", "d+", "e",  "f",  "f+", "g", "g+", "a", "b-", "b",  "r"},   // FMaj
  {"c",  "c+", "d", "e-", "e",  "f",  "f+", "g", "g+", "a", "b-", "b",  "r"},   // BfMaj
  {"c",  "c+", "d", "e-", "e",  "f",  "f+", "g", "a-", "a", "b-", "b",  "r"},   // EfMaj
  {"c",  "d-", "d", "e-", "e",  "f",  "f+", "g", "a-", "a", "b-", "b",  "r"},   // AfMaj
  {"c",  "d-", "d", "e-", "e",  "f",  "g-", "g", "a-", "a", "b-", "b",  "r"},   // DfMaj
  {"c",  "d-", "d", "e-", "e",  "f",  "g-", "g", "a-", "a", "b-", "c-", "r"},   // GfMaj
  {"c",  "d-", "d", "e-", "f-", "f",  "g-", "g", "a-", "a", "b-", "c-", "r"},   // CfMaj
  {"c",  "c+", "d", "d+", "e",  "f",  "f+", "g", "g+", "a", "a+", "b",  "r"},   // Amin
  {"c",  "c+", "d", "d+", "e",  "f",  "f+", "g", "g+", "a", "a+", "b",  "r"},   // Emin
  {"c",  "c+", "d", "d+", "e",  "f",  "f+", "g", "g+", "a", "a+", "b",  "r"},   // Bmin
  {"c",  "c+", "d", "d+", "e",  "f",  "f+", "g", "g+", "a", "a+", "b",  "r"},   // Fsmin
  {"c",  "c+", "d", "d+", "e",  "f",  "f+", "g", "g+", "a", "a+", "b",  "r"},   // Csmin
  {"c",  "c+", "d", "d+", "e",  "f",  "f+", "g", "g+", "a", "a+", "b",  "r"},   // Gsmin
  {"c",  "c+", "d", "d+", "e",  "e+", "f+", "g", "g+", "a", "a+", "b",  "r"},   // Dsmin
  {"b+", "c+", "d", "d+", "e",  "e+", "f+", "g", "g+", "a", "a+", "b",  "r"},   // Asmin
  {"c",  "c+", "d", "d+", "e",  "f",  "f+", "g", "g+", "a", "b-", "b",  "r"},   // Dmin
  {"c",  "c+", "d", "e-", "e",  "f",  "f+", "g", "g+", "a", "a+", "b",  "r"},   // Gmin
  {"c",  "c+", "d", "e-", "e",  "f",  "f+", "g", "a-", "a", "a+", "b",  "r"},   // Cmin
  {"c",  "d-", "d", "e-", "e",  "f",  "f+", "g", "a-", "a", "a+", "b",  "r"},   // Fmin
  {"c",  "d-", "d", "e-", "e",  "f",  "g-", "g", "a-", "a", "a+", "b",  "r"},   // Bfmin
  {"c",  "d-", "d", "e-", "e",  "f",  "g-", "g", "a-", "a", "a+", "c-", "r"},   // Efmin
  {"c",  "d-", "d", "e-", "f-", "f",  "g-", "g", "a-", "a", "a+", "c-", "r"}    // Afmin
};


/* void show_events(elist_t **seq) */
/* { */
/*   int i; */

/*   for (i=0; i<config.track.tracksize; i++) { */

/*     printf("\n\n====================<part %u>=============================\n", num+1); */
/*     puts(" measure | miditick | mmltick |\t\tdata"); */
/*     puts("---------+----------+---------+-----------------------------"); */

/*     for ( ; seq[i]!=NULL; seq[i]=seq[i]->next) { */
    
/*       switch (seq[i]->type) { */
/*       case RHYTHM: */
/* 	printf("  %4u\t |   %4u   |  %4u   | [rhythm] 拍子:%u/%u beatclock:%u hdm_semi:%u 1小節中のtick:MIDI:%u MML:%u\n", seq[i]->rhythm->pos.measure, seq[i]->rhythm->pos.miditick, seq[i]->rhythm->pos.mmltick, seq[i]->rhythm->num, 1<<seq[i]->rhythm->denom_pow2, seq[i]->rhythm->beatclock, seq[i]->rhythm->hdm_semi, seq[i]->rhythm->midimes_tick, seq[i]->rhythm->mmlmes_tick); */
/* 	break; */
/*       case MARKER: */
/* 	printf("  %4u\t |   %4u   |  %4u   | [marker] text:%s\n", seq[i]->marker->pos.measure, seq[i]->marker->pos.miditick, seq[i]->marker->pos.mmltick, seq[i]->marker->data); */
/* 	break; */
/*       case TEMPO: */
/* 	printf("  %4u\t |   %4u   |  %4u   | [tempo] bpm:%u\n", seq[i]->tempo->pos.measure, seq[i]->tempo->pos.miditick, seq[i]->tempo->pos.mmltick, 60000000/seq[i]->tempo->data); */
/* 	break; */
/*       case KEYSIG: */
/* 	printf("  %4u\t |   %4u   |  %4u   | [key signature] sig_num:%d minorflg:%u\n", seq[i]->keysig->pos.measure, seq[i]->keysig->pos.miditick, seq[i]->keysig->pos.mmltick, seq[i]->keysig->sig_num, seq[i]->keysig->minorflg); */
/* 	break; */
/*       case CCPC: */
/* 	if (seq[i]->ccpc->type == PC) { */
/* 	  printf("  %4u\t |   %4u   |  %4u   | [program change] no:%u\n", seq[i]->ccpc->pos.measure, seq[i]->ccpc->pos.miditick, seq[i]->ccpc->pos.mmltick, seq[i]->ccpc->value); */
/* 	} */
/* 	else if (seq[i]->ccpc->type == VOLUME) { */
/* 	  printf("  %4u\t |   %4u   |  %4u   | [control change: volume] value:%u\n", seq[i]->ccpc->pos.measure, seq[i]->ccpc->pos.miditick, seq[i]->ccpc->pos.mmltick, seq[i]->ccpc->value); */
/* 	} */
/* 	else if (seq[i]->ccpc->type == PAN) { */
/* 	  printf("  %4u\t |   %4u   |  %4u   | [control change: pan] value:%u\n", seq[i]->ccpc->pos.measure, seq[i]->ccpc->pos.miditick, seq[i]->ccpc->pos.mmltick, seq[i]->ccpc->value); */
/* 	} */
/* 	break; */
/*       case NOTE: */
/* 	if (seq[i]->note->velocity != 0) { */
/* 	  printf("  %4u\t |   %4u   |  %4u   | key:%4u 終了位置:MIDI:%u-%u MML:%u-%u velocity:%u\n", seq[i]->note->pos.measure, seq[i]->note->pos.miditick, seq[i]->note->pos.mmltick, seq[i]->note->key, seq[i]->note->endpos.measure, seq[i]->note->endpos.miditick, seq[i]->note->endpos.measure, seq[i]->note->endpos.mmltick, seq[i]->note->velocity); */
/* 	} */
/* 	else { */
/* 	  printf("  %4u\t |   %4u   |  %4u   | key:REST 終了位置:MIDI%u-%u MML:%u-%u\n", seq[i]->note->pos.measure, seq[i]->note->pos.miditick, seq[i]->note->pos.mmltick, seq[i]->note->endpos.measure, seq[i]->note->endpos.miditick, seq[i]->note->endpos.measure, seq[i]->note->endpos.mmltick); */
/* 	} */
/* 	break; */
/*       } */
    
/*     } */

/*   } */
  
/*   return; */
/* } */


char *make_outfilename(char *srcfilename)
{
  int filename_len, name_len;
  char *filename=NULL, *outfilename=NULL;
  
  name_len = strlen(srcfilename) - strlen(strrchr(srcfilename, '.'));

  if (config.mml.mml_extension == NULL) {
    filename_len = name_len;
    outfilename = (char *)malloc(sizeof(char)*(filename_len+1));
    if (outfilename == NULL) {
      EPRINTM();
      free_config();
      ERREXIT();
    }
    strncpy(outfilename, srcfilename, name_len);
    outfilename[name_len] = '\0';
  }
  else {
    filename_len = name_len + 1 + strlen(config.mml.mml_extension);
    outfilename = (char *)malloc(sizeof(char)*(filename_len+1));
    if (outfilename == NULL) {
      EPRINTM();
      free_config();
      ERREXIT();
    }
    strncpy(outfilename, srcfilename, name_len+1);
    outfilename[name_len+1] = '\0';
    strcat(outfilename, config.mml.mml_extension);
  }

  return outfilename;
}


int isoutput(char *filename)
{
  FILE *tmp;
  int ret=-1;
  
  tmp = fopen(filename, "r");
  if (tmp == NULL) {
    ret = 1;
  }
  else {
    while (ret == -1) {
      puts(" >> 出力するファイルと同名のファイルが存在します。上書きしますか\?");
      printf(" >> [1]はい\t[2]いいえ\t_");
      switch (getche()) {
      case '1':
	ret = 1;
	break;
      case '2':
	ret = 0;
	break;
      default:
	fprintf(stderr, "\n >>> 正しい値を入力してください。\n");
	break;
      }
    }
    puts("");
  }
  
  fclose(tmp);
  
  return ret;
}


void print_events(char *filename, elist_t **seq, unsigned int t_partnum)
{
  int i;
  elist_t *head;

  alloclentable();
  setlentable();

  fp = fopen(filename, "w");
  if (fp == NULL) {
    fprintf(stderr, "[ERROR] \"%s\"に書き込めません。\n", filename);
    fclose(fp);
    free_config();
    ERREXIT();
  }

  if (config.mml.print_timebase == 1) {
    switch (config.mml.mml_style) {
    case FMP7:
      fprintf(fp, "\'{ ClockCount=%u }\n\n", config.mml.timebase_mml);
      break;
    case NRTDRV:
      fprintf(fp, "#COUNT\t%u\n", config.mml.timebase_mml);
      if (config.ccpc.volume_nrtdrv == 0) {
	fprintf(fp, "#V_STEP\t%u\n", config.ccpc.vstep_nrtdrv);
      }
      fprintf(fp, "\n");
      break;
    case PMD:
      if (config.mml.timebase_pmd == 0) {
	fprintf(fp, "#Zenlen\t%u\n\n", config.mml.timebase_mml);
      }
      break;
    case FMP:
    case MXDRV:
      break;
    }
  }

  for (i=0; i<config.track.tracksize; i++) {
    read_seqdata(seq[i], t_partnum, i);
  }

  if (config.tempo.make_tempopart == 1 && t_partnum == 0) {
    read_seqdata(t_part, t_partnum, -1);
  }

  fclose(fp);

  releaselentable();
  
  return;
}


void alloclentable(void)
{
  lentable = (struct _LENDATA *)calloc(config.mml.timebase_mml, sizeof(struct _LENDATA));
  if (lentable == NULL) {
    EPRINTM();
    free_config();
    ERREXIT();
  }

  return;
}


void setlentable(void)
{
  int i, cnt=0;
  unsigned int len;
  int index;


  if (config.note.range_notelength == 0) {
    len = 1;
    while (config.mml.timebase_mml % len == 0) {   // count number of elements
      cnt++;
      len <<= 1;
    }
    cnt *= 2;
  }
  else {
    cnt = (config.note.range_notelength + 1) * 2 - 1;
  }

  lenmk = (struct _LENMARK *)malloc(cnt*sizeof(struct _LENMARK));
  if (lenmk == NULL) {
    EPRINTM();
    free_config();
    ERREXIT();
  }

  
  len = 1;
  for (i=0; i<cnt; i++) {   // set element notes
    index = setlenmk(len);
    setlendata1(lenmk[index].gate-1, 0, NULL, len, 0);
    i++;
    if ( config.note.range_notelength == 0 ||
	 (config.note.range_notelength > 0 && i < cnt) ) {
      len *= 3;
      index = setlenmk(len);
      setlendata1(lenmk[index].gate-1, 0, NULL, len, 0);
      len = (len / 3) << 1;
    }
  }


  if (config.note.notelength_style == 0) {
    for (i=1; i<cnt; i++) {   // combinate element notes
      loop_setlentable(i+1, cnt, lenmk[i].gate-1, 1);
    }
    if (config.note.range_notelength > 0) {
      for (i=0; i<config.mml.timebase_mml; i++) {
	if (lentable[i].size == 0) {
	  setremainlendata(i);
	}
      }
    }
  }
  else {
    for (i=1; i<config.mml.timebase_mml; i++) {
      if (lentable[i].size == 0) {
        setlendata2(i, cnt);
      }
    }
  }


  FREE(lenmk);

  
  return;
}


int setlenmk(unsigned int len)
{
  static int size=0;
  int cmp, inflg=0, ret;

  if (size == 0) {
    lenmk[0].len = len;
    lenmk[0].gate = config.mml.timebase_mml / len;
    ret = 0;
  }
  else  {
    cmp = size - 1;
    
    do {
      if (lenmk[cmp].len < len) {
	lenmk[cmp+1].len = len;
	lenmk[cmp+1].gate = config.mml.timebase_mml / len;
	inflg = 1;
	ret = cmp+1;
	break;
      }
      else {
	lenmk[cmp+1].len = lenmk[cmp].len;
	lenmk[cmp+1].gate = lenmk[cmp].gate;
	cmp -= 1;
      }
    } while (cmp >= 0);
    if (inflg == 0) {
      lenmk[0].len = len;
      lenmk[0].gate = config.mml.timebase_mml / len;
      ret = 0;
    }

  }

  size++;

  return ret;
}


void loop_setlentable(int n, int cnt, int preindex, int size)
{
  int i;
  int index, eval;

  for (i=n; i<cnt; i++) {
    index = preindex+lenmk[i].gate;
    if (index < config.mml.timebase_mml) {
      if (lentable[index].size == 0) {
      	setlendata1(index, size, lentable[preindex].data, lenmk[i].len, lentable[preindex].eval);
      	loop_setlentable(i+1, cnt, index, lentable[index].size);
      }
      else if (lentable[index].size > size + 1) {
        FREE(lentable[index].data);
        setlendata1(index, size, lentable[preindex].data, lenmk[i].len, lentable[preindex].eval);
        loop_setlentable(i+1, cnt, index, lentable[index].size);
      }
      else if (lentable[index].size == size + 1) {
	if (lenmk[i].len % 3 > 0) {
	  eval = lentable[preindex].eval + 1;
	}
	else {
	  eval = lentable[preindex].eval - 1;
	}	
	if (lentable[index].eval < eval) {
	  FREE(lentable[index].data);
	  setlendata1(index, size, lentable[preindex].data, lenmk[i].len, lentable[preindex].eval);
	}
        loop_setlentable(i+1, cnt, index, lentable[index].size);
      }
      else {
        loop_setlentable(i+1, cnt, index, lentable[index].size);
      }
    }
  }

  return;
}


void setlendata1(unsigned int index, int size, int *predata, int adddata, int preeval)
{
  int i;
  int *dottmp, dotlast, dotflg=0, dotcnt=0; 

  lentable[index].size = size + 1;

  lentable[index].data = (int *)malloc((size+1)*sizeof(int));
  if (lentable[index].data == NULL) {
    EPRINTM();
    free_config();
    ERREXIT();
  }

  for (i=0; i<size; i++) {
    lentable[index].data[i] = predata[i];
  }
  if (config.note.dot_enable == 1) {
    dottmp = &adddata;
    for (i=size-1; i>=0; i--) {
      if (lentable[index].data[i] == *dottmp * 2) {
  	dottmp = &lentable[index].data[i];
  	if (dotcnt == 0) {
  	  dotlast = i;
  	}
  	dotcnt++;
      }
      else if (dotcnt > 0) {
  	dotflg = 1;
  	break;   //  add after dotlast
      }
      if (config.note.dot_length > 0 && dotcnt == config.note.dot_length) {
  	break;   // add last array
      }
    }
    if (dotflg == 0) {
      i = size;
    }
    else {
      for (i=size-1; i>dotlast; i--) {
  	lentable[index].data[i+1] = lentable[index].data[i];
      }
      i = dotlast + 1;
    }
  }
  lentable[index].data[i] = adddata;

  if (adddata % 3 > 0) {
    lentable[index].eval = preeval + 1;
  }
  else {
    lentable[index].eval = preeval - 1;
  }

  return;
}


void setremainlendata(int n)
{
  int i;

  if (n == 0 || lentable[n-1].size == 0) {
    lentable[n].remain = n + 1;
  }
  else {
    lentable[n].size = lentable[n-1].size;
    
    lentable[n].data = (int *)malloc(lentable[n].size*sizeof(int));
    if (lentable[n].data == NULL) {
      EPRINTM();
      free_config();
      ERREXIT();
    }
    
    for (i=0; i<lentable[n].size; i++) {
      lentable[n].data[i] = lentable[n-1].data[i];   
    }
    lentable[n].remain = lentable[n-1].remain + 1;
  }

  return;
}


void setlendata2(int index, int cnt)
{
  int i;
  unsigned int gate=0;

  lentable[index].data = (int *)malloc(cnt*sizeof(int));
  if (lentable[index].data == NULL) {
    EPRINTM();
    free_config();
    ERREXIT();
  }

  for (i=0; i<cnt; i++) {
    if ( lenmk[i].len % 3 > 0 && (gate + lenmk[i].gate) <= (index + 1) ) {
      lentable[index].data[lentable[index].size] = lenmk[i].len;
      lentable[index].size++;
      gate += lenmk[i].gate;
      if (gate == index + 1) {
	return;
      }
    }
  }

  for (i=0; i<cnt; i++) {
    if ( lenmk[i].len % 3 == 0 && (gate + lenmk[i].gate) <= (index + 1) ) {
      lentable[index].data[lentable[index].size] = lenmk[i].len;
      lentable[index].size++;
      gate += lenmk[i].gate;
      if (gate == index + 1) {
	return;
      }
    }
  }

  lentable[index].remain = index + 1 - gate;

  return;
}


void releaselentable(void)
{
  int i;

  for (i=0; i<config.mml.timebase_mml; i++) {
    if (lentable[i].size > 0) {
      FREE(lentable[i].data);
    }
  }
  FREE(lentable);

  return;
}



void read_seqdata(elist_t *seq, unsigned int t_partnum, int num)
{
  int namelen, indexflg=0;
  /* marker_t *marker=NULL; */
  elist_t *event;


  event = seq;
  lastflg = 0;
  mescnt = 0;
  premescnt = 0;
  mesflg = 1;
  octaveflg = 1;
  rhythm_now = NULL;
  

  if (seq == t_part && config.track.print_partname != 0) {
    tempopartflg = 1;
    print_partname();
  }
  else {
    tempopartflg = 0;
    if (config.track.print_partname != 0) {
      print_partname();
    }
    if (partindex == 0) {
      switch (config.mml.mml_style) {
      case CUSTOM:
      case FMP7:
      case MXDRV:
      case NRTDRV:
	break;
      case FMP:
	if (config.mml.print_timebase == 1) {
	  fprintf(fp, "C%u ", config.mml.timebase_mml);
	}
	break;
      case PMD:
	if (config.mml.print_timebase == 1 && config.mml.timebase_pmd == 1) {
	  fprintf(fp, "C%u ", config.mml.timebase_mml);
	}
	break;
      }
    }
    indexflg = 1;
  }
  
    
  for ( ; event!=NULL; event=event->next) {

    if (event->next == NULL) {
      lastflg = 1;
    }
    
    switch (event->type) {
    case RHYTHM:
      rhythm_now = event->rhythm;
      if (config.mml.newline_rhythm == 1 && mesflg == 0 && lastflg == 0) {
	fprintf(fp, "\n");
	if (config.track.print_partname != 0) {
	  print_partname();
	}
	noteeventflg = 0;
	mesflg = 1;
	octaveflg = 1;
      }
      break;
    case MARKER:
      /* marker = event->marker;
	 noteeventflg = 0; */
      break;
    case TEMPO:
      if (t_partnum == num+1) {
	print_tempo(event->tempo);
      }
      noteeventflg = 0;
      break;
    case KEYSIG:
      set_keysig(event->keysig);
      break;
    case CCPC:
      print_ccpc(event->ccpc);
      noteeventflg = 0;
      break;
    case NOTE:
      print_note(event->note);
      break;
    }

  }   // for (event)
  
  
  if (indexflg == 1 && num != -1) {
    fprintf(fp, "\n\n");
    partindex++;
  }
  
  
  return;
}


void print_partname(void)
{
  if (tempopartflg == 1) {
    fprintf(fp, "%s ", config.track.name_tempopart);
  }
  else {
    fprintf(fp, "%s ", config.track.name_part[partindex]);
  }

  return;
}


void print_tempo(tempo_t *tempo)
{
  unsigned int value=60000000/tempo->data;
  
  if (noteeventflg == 1) {
    fprintf(fp," ");
  }
  switch (config.mml.mml_style) {
  case CUSTOM:
    fprintf(fp, "%s%d ", config.tempo.tempo_cmd, value);
    break;
  case FMP7:
  case FMP:
    fprintf(fp, "T%d ", value);
    break;
  case PMD:
  case MXDRV:
  case NRTDRV:
    fprintf(fp, "t%d ", value);
    break;
  }
  
  return;
}


void set_keysig(keysig_t *data)
{
  if (data->minorflg == 0) {
    switch (data->sig_num) {
    case -7: keysigtype = CfMaj; break;
    case -6: keysigtype = GfMaj; break;
    case -5: keysigtype = DfMaj; break;
    case -4: keysigtype = AfMaj; break;
    case -3: keysigtype = EfMaj; break;
    case -2: keysigtype = BfMaj; break;
    case -1: keysigtype = FMaj;  break;
    case  0: keysigtype = CMaj;  break;
    case  1: keysigtype = GMaj;  break;
    case  2: keysigtype = DMaj;  break;
    case  3: keysigtype = AMaj;  break;
    case  4: keysigtype = EMaj;  break;
    case  5: keysigtype = BMaj;  break;
    case  6: keysigtype = FsMaj; break;
    case  7: keysigtype = CsMaj; break;
    }
  }
  else {
    switch (data->sig_num) {
    case -7: keysigtype = Afmin; break;
    case -6: keysigtype = Efmin; break;
    case -5: keysigtype = Bfmin; break;
    case -4: keysigtype = Fmin;  break;
    case -3: keysigtype = Cmin;  break;
    case -2: keysigtype = Gmin;  break;
    case -1: keysigtype = Dmin;  break;
    case  0: keysigtype = Amin;  break;
    case  1: keysigtype = Emin;  break;
    case  2: keysigtype = Bmin;  break;
    case  3: keysigtype = Fsmin; break;
    case  4: keysigtype = Csmin; break;
    case  5: keysigtype = Gsmin; break;
    case  6: keysigtype = Dsmin; break;
    case  7: keysigtype = Asmin; break;
    }
  }

  return;
}


void print_ccpc(ccpc_t *ccpc)
{
  if (noteeventflg == 1) {
    fprintf(fp, " ");
  }
  fprintf(fp, "%s ", ccpc->modvalue);

  return;
}


void print_note(note_t *note)
{
  if (note->velocity > 0) {
    isoctave(note->key);
    isnewmes(KEYNOTE, note->pos, note->endpos, note->key%12, note->tieflg);
  }
  else {
    isnewmes(RESTNOTE, note->pos, note->endpos, 12, note->tieflg);
  }
  
  return;
}


void isoctave(unsigned char key)
{
  int i;
  int octave, octdif;
  
  octave = (key / 12) - 1;
  if (config.note.octave_newline == 1 && octaveflg == 1) {
    if (noteeventflg == 1) {
      fprintf(fp, " ");
    }
    switch (config.mml.mml_style) {
    case CUSTOM:
      fprintf(fp, "%s%d ", config.note.octave_sign, octave);
      break;
    case FMP7:
    case FMP:
    case PMD:
    case MXDRV:
    case NRTDRV:
      fprintf(fp, "o%d ", octave);
      break;
    }
    mesflg = 0;
    octaveflg = 0;
    preoctave = octave;
  }
  else {
    octdif = octave - preoctave;
    if (octdif > 0) {
      for (i=0; i<octdif; i++) {
	if (config.note.octave_direction == 0) {
	  fprintf(fp, ">");
	}
	else {
	  fprintf(fp, "<");
	}
      }
    }
    else if (octdif < 0) {
      for (i=0; i>octdif; i--) {
	if (config.note.octave_direction == 0) {
	  fprintf(fp, "<");
	}
	else {
	  fprintf(fp, ">");
	}
      }
    }
    preoctave = octave;
  }

  return;
}


void isnewmes(enum _NOTETYPE type, pos_t pos, pos_t endpos, unsigned char key, int tieflg)
{
  if (cmppos(endpos, rhythm_now->endpos) < 0) {   // before rhythm change
    mescnt += endpos.measure - pos.measure;
    if (mescnt != premescnt) {
      if (config.mml.newline_mes > 0 && mescnt % config.mml.newline_mes == 0) {   // new line
  	mesflg = 1;
      }
      else {   // new block
  	mesflg = 2;
      }
      if (config.note.tienote_block == 0) {   // add after tie
	switch (tieflg) {
	case 3:
	case 4:
	case 5:
	  if (type == KEYNOTE ||
	      (type == RESTNOTE && config.note.resttie_enable == 1) ) {
	    mesflg *= -1;
	  }
	  break;
	default:
	  break;
	}
      }
    }
    else {
      mesflg = ((mesflg < 0)? (mesflg * -1) : 0);
    }
  }
  else if (cmppos(endpos, rhythm_now->endpos) == 0) {   // when just new rhythm and rhythm endpos-tick is 0
    if (config.mml.newline_rhythm == 1) {
      mescnt = 0;
      if (config.note.tienote_block == 0) {   // add after tie
	switch (tieflg) {
	case 3:
	case 4:
	case 5:
	  if (type == KEYNOTE ||
	      (type == RESTNOTE && config.note.resttie_enable == 1) ) {
	    mesflg = -1;
	  }
	  else {
	    mesflg = 0;
	  }
	  break;
	default:
	  mesflg = 0;
	  break;
	}
      }
      else {
	mesflg = 0;
      }
    }
    else {
      mescnt += endpos.measure - pos.measure;
      if (config.mml.newline_mes > 0 && mescnt % config.mml.newline_mes == 0) {   // new line
  	mesflg = 1;
      }
      else {   // new block
  	mesflg = 2;
      }
      if (config.note.tienote_block == 0) {   // add after tie
	switch (tieflg) {
	case 3:
	case 4:
	case 5:
	  if (type == KEYNOTE ||
	      (type == RESTNOTE && config.note.resttie_enable == 1) ) {
	    mesflg *= -1;
	  }
	  break;
	default:
	  break;
	}
      }
    }
  }
  else if (endpos.measure == rhythm_now->endpos.measure+1 && endpos.miditick == 0 && rhythm_now->endpos.miditick > 0) {   // when just new rhythm and rhythm endpos-tick is not 0
    if (config.mml.newline_rhythm == 1) {
      mescnt = 0;
      if (config.note.tienote_block == 0) {   // add after tie
	switch (tieflg) {
	case 3:
	case 4:
	case 5:
	  if (type == KEYNOTE ||
	      (type == RESTNOTE && config.note.resttie_enable == 1) ) {
	    mesflg = -1;
	  }
	  else {
	    mesflg = 0;
	  }
	  break;
	default:
	  mesflg = 0;
	  break;
	}
      }
      else {
	mesflg = 0;
      }
    }
    else {
      mescnt += endpos.measure - pos.measure;
      if (config.mml.newline_mes > 0 && mescnt % config.mml.newline_mes == 0) {   // new line
  	mesflg = 1;
      }
      else {   // new block
  	mesflg = 2;
      }
      if (config.note.tienote_block == 0) {   // add after tie
	switch (tieflg) {
	case 3:
	case 4:
	case 5:
	  mesflg *= -1;
	  break;
	default:
	  break;
	}
      }
    }
  }

  printconvnote(type, calcgate(pos, endpos), key, tieflg);

  if (lastflg == 0) {
    switch (mesflg) {
    case -2:
    case -1:
      noteeventflg = 1;
      break;
    case 0:   // not divide
      noteeventflg = 1;
      break;
    case 1:   // new line
      fprintf(fp, "\n");
      if (config.track.print_partname != 0) {
	print_partname();
      }
      octaveflg = 1;
      noteeventflg = 0;
      break;
    case 2:   // space or tab
      switch (config.mml.newblock_mes) {
      case 0:
	break;
      case 1:
	fprintf(fp," ");
	break;
      case 2:
	fprintf(fp,"\t");
	break;
      }
      mesflg = 0;
      noteeventflg = 0;
      break;
    }
  }

  premescnt = mescnt;   // for insert space or new line

  return;
}


struct _GATEDATA calcgate(pos_t pos, pos_t endpos)
{
  struct _GATEDATA data;
  rhythm_t *rhythm=rhythm_now;

  data.wholecnt = 0;
  data.remaingate = 0;
  
  while (1) {
    if (pos.measure == endpos.measure) {
      data = addgate(data, pos.mmltick, endpos.mmltick);
      break;
    }
    else {
      if (pos.measure == rhythm->endpos.measure) {
	data = addgate(data, pos.mmltick, rhythm->endpos.mmltick);
	rhythm = rhythm->next;
      }
      else if (pos.measure + 1 == rhythm->endpos.measure && rhythm->endpos.mmltick == 0) {
	data = addgate(data, pos.mmltick, rhythm->mmlmes_tick);
	rhythm = rhythm->next;
      }
      else {
	data = addgate(data, pos.mmltick, rhythm->mmlmes_tick);
      }
      pos.measure++;
      pos.mmltick = 0;
    }
  }

  return data;
}


struct _GATEDATA addgate(struct _GATEDATA data, unsigned int start, unsigned int end)
{
  data.remaingate += end - start;
  while (data.remaingate >= config.mml.timebase_mml) {
    data.wholecnt++;
    data.remaingate -= config.mml.timebase_mml;
  }

  return data;
}


void printconvnote(enum _NOTETYPE type, struct _GATEDATA gate, unsigned char key, int tieflg)
{
  int i=0, firstflg=1;

  while (1) {

    /*--- print tie for 1 note ---*/
    if ( (type == KEYNOTE ||
	  (type == RESTNOTE && config.note.resttie_enable == 1 && config.mml.mml_style != FMP) ) &&
	 firstflg == 0) {
      fprintf(fp, "%s", config.note.tie_cmd);
    }
  
    /*--- print note ---*/
    if (gate.wholecnt > 0) {
      printconvlen(type, key, config.mml.timebase_mml-1, tieflg, i);
      gate.wholecnt--;
      if (gate.wholecnt == 0 && gate.remaingate == 0) {
	break;
      }
    }
    else {
      printconvlen(type, key, gate.remaingate-1, tieflg, i);
      break;
    }

    i++;
    firstflg = 0;

  }

  /*--- print tie for over mesure or insert ccpc ---*/
  switch (tieflg) {
  case 3:
  case 4:
  case 5:
    if (type == KEYNOTE ||
	(type == RESTNOTE && config.note.resttie_enable == 1 && config.mml.mml_style != FMP)) {
      fprintf(fp, "%s", config.note.tie_cmd);
    }
    break;
  case 6:
  case 7:
  case 8:
    if (type == KEYNOTE) {
      switch (config.mml.mml_style) {
      case CUSTOM:
      case FMP7:
      case FMP:
      case PMD:
      case MXDRV:
	fprintf(fp, "%s", config.note.tie_cmd);
	break;
      case NRTDRV:
	fprintf(fp, "&");   // next is ccpc-tie
	break;
      }
    }
    break;
  default:
    break;
  }

  return;
}


void printconvlen(enum _NOTETYPE type, unsigned char key, unsigned int index, int tieflg1, int tieflg2)
{
  int i=0, cnt=0;
  int tmp;

  if (lentable[index].size > 0) {
    tmp = lentable[index].data[0];
    for (i=0; i<lentable[index].size; i++) {
      if (config.note.dot_enable == 1 && i > 0 &&
	  tmp == lentable[index].data[i] / 2 &&
	  cnt > 0) {
	fprintf(fp, ".");
	if (config.note.dot_length > 0) {
	  cnt = (cnt + 1) % (config.note.dot_length + 1);
	}
	else {
	  cnt++;
	}
      }
      else {
	if ( (type == KEYNOTE ||
	      type == RESTNOTE && config.note.resttie_enable == 1 && config.mml.mml_style != FMP) && 
	     i > 0) {
	  fprintf(fp, "%s", config.note.tie_cmd);
	}
	if ( (type == KEYNOTE ||
	      (type == RESTNOTE && config.note.resttie_enable == 1 && config.mml.mml_style != FMP) ) &&
	     config.note.extend_note == 1) {
	  if (tieflg2 > 0 || i > 0) {
	    fprintf(fp, "%d", lentable[index].data[i]);
	  }
	  else if (i == 0) {
	    switch (tieflg1) {
	    case 1:
	    case 4:
	    case 7:
	      fprintf(fp, "%d", lentable[index].data[i]);
	      break;
	    default:
	      fprintf(fp, "%s%d", keytable[keysigtype][key], lentable[index].data[i]);
	      break;
	    }
	  }
	  else {
	    fprintf(fp, "%s%d", keytable[keysigtype][key], lentable[index].data[i]);
	  }
	}
	else {
	  fprintf(fp, "%s%d", keytable[keysigtype][key], lentable[index].data[i]);
	}
	cnt = 1;
      }
      tmp = lentable[index].data[i];
    }
  }

  if (lentable[index].remain > 0) {
    if ( (type == KEYNOTE ||
	  (type == RESTNOTE && config.note.resttie_enable == 1 && config.mml.mml_style != FMP) ) &&
	 i > 0) {
      fprintf(fp, "%s", config.note.tie_cmd);
    }
    if ( (type == KEYNOTE ||
	  (type == RESTNOTE && config.note.resttie_enable == 1 && config.mml.mml_style != FMP) ) &&
	 config.note.extend_note == 1) {
      if (tieflg2 > 0 || i > 0) {
	fprintf(fp, "%d", lentable[index].data[i]);
      }
      else if (i == 0) {
	switch (tieflg1) {
	case 1:
	case 4:
	case 7:
	  fprintf(fp, "%s%d", config.note.clockcount_sign, lentable[index].remain);
	  break;
	default:
	  fprintf(fp, "%s%s%d", keytable[keysigtype][key], config.note.clockcount_sign, lentable[index].remain);
	  break;
	}
      }
      else {
	fprintf(fp, "%s%s%d", keytable[keysigtype][key], config.note.clockcount_sign, lentable[index].remain);
      }
    }
    else {
      fprintf(fp, "%s%s%d", keytable[keysigtype][key], config.note.clockcount_sign, lentable[index].remain);
    }    
  }

  return;
}
