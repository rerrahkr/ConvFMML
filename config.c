#include <string.h>
#include "convfmml.h"
#include "config.h"


void load_config(char *path)
{
  int pathlen;
  char buf[BUF_SIZE];
  FILE *fp;

  init_config();

  pathlen = strlen(path);
  path[pathlen-3] = 'i';
  path[pathlen-2] = 'n';
  path[pathlen-1] = 'i';

  fp = fopen(path, "r");
  if (fp == NULL) {
    fprintf(stderr, "[ERROR] \"ConvFMML.ini\"の読み込みに失敗しました。\n");
    fclose(fp);
    free_config();
    ERREXIT();
  }

  while (!feof(fp)) {
    fgets(buf, BUF_SIZE, fp);
    strtok(buf, "\n\0");
    set_config(buf);
  }

  fclose(fp);

  if (check_config()) {
    fclose(fp);
    free_config();
    ERREXIT();
  }
  
  set_modecmds();
  set_track_and_partsize();
  if (config.track.print_partname == 1) {
    set_auto_partname();
  }

  return;
}


void init_config(void)
{
  int i;
  
  config.mml.mml_style = -1;
  config.mml.mml_extension = NULL;
  config.mml.fmp_extension = -1;
  config.mml.timebase_mml = 0;
  config.mml.print_timebase = -1;
  config.mml.newblock_mes = -1;
  config.mml.newline_mes = -1;
  config.mml.newline_rhythm = -1;
  
  config.note.octave_newline = -1;
  config.note.octave_sign = NULL;
  config.note.octave_direction = -1;
  config.note.notelength_style = -1;
  config.note.range_notelength = -1;
  config.note.clockcount_sign = NULL;
  config.note.dot_enable = -1;
  config.note.dot_length = -1;
  config.note.measurecut_note = -1;
  config.note.tie_cmd = NULL;
  config.note.resttie_enable = -1;
  config.note.extend_note = -1;
  config.note.tienote_block = -1;

  config.ccpc.overlap_ccpc = -1;
  config.ccpc.invalid_ccpc = -1;
  config.ccpc.replace_ccpc = -1;
  config.ccpc.omit_ccpc = -1;
  config.ccpc.volume_enable = -1;
  config.ccpc.volume_cmd = NULL;
  config.ccpc.volume_range = -1;
  config.ccpc.volume_pmd = -1;
  config.ccpc.volume_mxdrv = -1;
  config.ccpc.volume_nrtdrv = -1;
  config.ccpc.vstep_nrtdrv = -1;
  config.ccpc.pan_enable = -1;
  config.ccpc.pan_cmdmode = -1;
  config.ccpc.pan_midicmd = NULL;
  config.ccpc.pan_lcmd = NULL;
  config.ccpc.pan_ccmd = NULL;
  config.ccpc.pan_rcmd = NULL;
  config.ccpc.pan_fmp7 = -1;
  config.ccpc.pan_left = -1;
  config.ccpc.pan_right = -1;
  config.ccpc.pc_enable = -1;
  config.ccpc.pc_cmd = NULL;

  config.tempo.tempo_enable = -1;
  config.tempo.tempo_cmd = NULL;
  config.tempo.make_tempopart = -1;
  
  config.track.print_track = -1;
  config.track.tracksize = 0;
  memset(config.track.number_track, 0, sizeof(int)*PART_SIZE);
  config.track.print_partname = -1;
  config.track.partsize = 0;
  config.track.fmp7_autoname = -1;
  config.track.fmp_autoname = -1;
  config.track.pmd_autoname = -1;
  config.track.nrtdrv_autoname = -1;
  memset(config.track.name_part, 0, PART_SIZE);
  config.track.name_tempopart = NULL;

  return;
}


void set_config(char *buf)
{
  char newbuf[BUF_SIZE];
  
  /*--- config.mml ---*/
  if (!STRCMP("mml_style")) {
    sscanf(buf, "mml_style=%d", &config.mml.mml_style);
  }
  else if (!STRCMP("mml_extension")) {
    if (config.mml.mml_style == CUSTOM) {
      if (sscanf(buf, "mml_extension=%s", newbuf) != EOF) {
	config.mml.mml_extension = sub_confstr(newbuf);
      }
    }
  }
  else if (!STRCMP("fmp_extension")) {
    if (config.mml.mml_style == FMP) {
      sscanf(buf, "fmp_extension=%d", &config.mml.fmp_extension);
    }
  }
  else if (!STRCMP("timebase_mml")) {
    sscanf(buf, "timebase_mml=%u", &config.mml.timebase_mml);
  }
  else if (!STRCMP("print_timebase")) {
    switch (config.mml.mml_style) {
    case CUSTOM:
    case MXDRV:
      break;
    case FMP7:
    case FMP:
    case PMD:
    case NRTDRV:
      sscanf(buf, "print_timebase=%d", &config.mml.print_timebase);
      break;
    default:   // wrong value
      break;
    }
  }
  else if (!STRCMP("timebase_pmd")) {
    if (config.mml.mml_style == PMD && config.mml.print_timebase == 1) {
      sscanf(buf, "timebase_pmd=%d", &config.mml.timebase_pmd);
    }
  }
  else if (!STRCMP("newblock_mes")) {
    sscanf(buf, "newblock_mes=%d", &config.mml.newblock_mes);
  }
  else if (!STRCMP("newline_mes")) {
    sscanf(buf, "newline_mes=%d", &config.mml.newline_mes);
  }
  else if (!STRCMP("newline_rhythm")) {
    sscanf(buf, "newline_rhythm=%d", &config.mml.newline_rhythm);
  }
  /*--- config.note ---*/
  else if (!STRCMP("octave_newline")) {
    sscanf(buf, "octave_newline=%d", &config.note.octave_newline);
  }
  else if (!STRCMP("octave_sign")) {
    if (config.mml.mml_style == CUSTOM) {
      if (sscanf(buf, "octave_sign=%s", newbuf) != EOF) {
	config.note.octave_sign = sub_confstr(newbuf);
      }
    }
  }
  else if (!STRCMP("octave_direction")) {
    sscanf(buf, "octave_direction=%d", &config.note.octave_direction);
  }
  else if (!STRCMP("notelength_style")) {
    sscanf(buf, "notelength_style=%d", &config.note.notelength_style);
  }
  else if (!STRCMP("range_notelength")) {
    sscanf(buf, "range_notelength=%d", &config.note.range_notelength);
  }
  else if (!STRCMP("clockcount_sign")) {
    if (config.mml.mml_style == CUSTOM && config.note.range_notelength > 0) {
      if (sscanf(buf, "clockcount_sign=%s", newbuf) != EOF) {
	config.note.clockcount_sign = sub_confstr(newbuf);
      }
    }
  }
  else if (!STRCMP("dot_enable")) {
    sscanf(buf, "dot_enable=%d", &config.note.dot_enable);
  }
  else if (!STRCMP("dot_length")) {
    if (config.note.dot_enable == 1) {
      sscanf(buf, "dot_length=%d", &config.note.dot_length);
    }
  }
  else if (!STRCMP("measurecut_note")) {
    sscanf(buf, "measurecut_note=%d", &config.note.measurecut_note);
  }
  else if (!STRCMP("tie_cmd")) {
    if (config.mml.mml_style == CUSTOM) {
      if (sscanf(buf, "tie_cmd=%s", newbuf) != EOF) {
	config.note.tie_cmd = sub_confstr(newbuf);
      }
    }
  }
  else if (!STRCMP("resttie_enable")) {
    sscanf(buf, "resttie_enable=%d", &config.note.resttie_enable);
  }
  else if (!STRCMP("extend_note")) {
    sscanf(buf, "extend_note=%d", &config.note.extend_note);
  }
  else if (!STRCMP("tienote_block")) {
    sscanf(buf, "tienote_block=%d", &config.note.tienote_block);
  }
  /*--- config.ccpc ---*/
  else if (!STRCMP("overlap_ccpc")) {
    sscanf(buf, "overlap_ccpc=%d", &config.ccpc.overlap_ccpc);
  }
  else if (!STRCMP("invalid_ccpc")) {
    sscanf(buf, "invalid_ccpc=%d", &config.ccpc.invalid_ccpc);
  }
  else if (!STRCMP("replace_ccpc")) {
    sscanf(buf, "replace_ccpc=%d", &config.ccpc.replace_ccpc);
  }
  else if (!STRCMP("omit_ccpc")) {
    sscanf(buf, "omit_ccpc=%d", &config.ccpc.omit_ccpc);
  }
  else if (!STRCMP("volume_enable")) {
    sscanf(buf, "volume_enable=%d", &config.ccpc.volume_enable);
  }
  else if (!STRCMP("volume_cmd")) {
    if (config.mml.mml_style == CUSTOM && config.ccpc.volume_enable == 1) {
      if (sscanf(buf, "volume_cmd=%s", newbuf) != EOF) {
	config.ccpc.volume_cmd = sub_confstr(newbuf);
      }
    }
  }
  else if (!STRCMP("volume_range")) {
    if (config.mml.mml_style == CUSTOM && config.ccpc.volume_enable == 1) {
      sscanf(buf, "volume_range=%d", &config.ccpc.volume_range);
    }
  }
  else if (!STRCMP("volume_pmd")) {
    if (config.mml.mml_style == PMD && config.ccpc.volume_enable == 1) {
      sscanf(buf, "volume_pmd=%d", &config.ccpc.volume_pmd);
    }
  }
  else if (!STRCMP("volume_mxdrv")) {
    if (config.mml.mml_style == MXDRV && config.ccpc.volume_enable == 1) {
      sscanf(buf, "volume_mxdrv=%d", &config.ccpc.volume_mxdrv);
    }
  }
  else if (!STRCMP("volume_nrtdrv")) {
    if (config.mml.mml_style == NRTDRV && config.ccpc.volume_enable == 1) {
      sscanf(buf, "volume_nrtdrv=%d", &config.ccpc.volume_nrtdrv);
    }
  }
  else if (!STRCMP("vstep_nrtdrv")) {
    if (config.mml.mml_style == NRTDRV && config.ccpc.volume_enable == 1 && config.ccpc.volume_nrtdrv == 0) {
      sscanf(buf, "vstep_nrtdrv=%d", &config.ccpc.vstep_nrtdrv);
    }
  }
  else if (!STRCMP("pan_enable")) {
    sscanf(buf, "pan_enable=%d", &config.ccpc.pan_enable);
  }
  else if (!STRCMP("pan_cmdmode")) {
    if (config.mml.mml_style == CUSTOM && config.ccpc.pan_enable == 1) {
      sscanf(buf, "pan_cmdmode=%d", &config.ccpc.pan_cmdmode);
    }
  }
  else if (!STRCMP("pan_midicmd")) {
    if (config.mml.mml_style == CUSTOM && config.ccpc.pan_enable == 1 && config.ccpc.pan_cmdmode == 0) {
      if (sscanf(buf, "pan_midicmd=%s", newbuf) != EOF) {
	config.ccpc.pan_midicmd = sub_confstr(newbuf);
      }
    }
  }
  else if (!STRCMP("pan_lcmd")) {
    if (config.mml.mml_style == CUSTOM && config.ccpc.pan_enable == 1 && config.ccpc.pan_cmdmode == 1) {
      if (sscanf(buf, "pan_lcmd=%s", newbuf) != EOF) {
	config.ccpc.pan_lcmd = sub_confstr(newbuf);
      }
    }
  }
  else if (!STRCMP("pan_ccmd")) {
    if (config.mml.mml_style == CUSTOM && config.ccpc.pan_enable == 1 && config.ccpc.pan_cmdmode == 1) {
      if (sscanf(buf, "pan_ccmd=%s", newbuf) != EOF) {
	config.ccpc.pan_ccmd = sub_confstr(newbuf);
      }
    }
  }
  else if (!STRCMP("pan_rcmd")) {
    if (config.mml.mml_style == CUSTOM && config.ccpc.pan_enable == 1 && config.ccpc.pan_cmdmode == 1) {
      if (sscanf(buf, "pan_rcmd=%s", newbuf) != EOF) {
	config.ccpc.pan_rcmd = sub_confstr(newbuf);
      }
    }
  }
  else if (!STRCMP("pan_fmp7")) {
    if (config.mml.mml_style == FMP7 && config.ccpc.pan_enable == 1) {
      sscanf(buf, "pan_fmp7=%d", &config.ccpc.pan_fmp7);
    }
  }
  else if (!STRCMP("pan_left")) {
    if (config.ccpc.pan_enable == 1) {
      switch (config.mml.mml_style) {
      case CUSTOM:
	if (config.ccpc.pan_cmdmode == 1) {
	  if (sscanf(buf, "pan_left=%s", newbuf) != EOF) {
	    set_panpoint(newbuf, LEFT);
	  }
	}
	break;
      case FMP7:
	break;
      case FMP:
      case PMD:
      case MXDRV:
      case NRTDRV:
	if (sscanf(buf, "pan_left=%s", newbuf) != EOF) {
	  set_panpoint(newbuf, LEFT);
	}
	break;
      default:   // wrong value
	break;
      }
    }
  }
  else if (!STRCMP("pan_right")) {
    if (config.ccpc.pan_enable == 1) {
      switch (config.mml.mml_style) {
      case CUSTOM:
	if (config.ccpc.pan_cmdmode == 1) {
	  if (sscanf(buf, "pan_right=%s", newbuf) != EOF) {
	    set_panpoint(newbuf, RIGHT);
	  }
	}
	break;
      case FMP7:
	break;
      case FMP:
      case PMD:
      case MXDRV:
      case NRTDRV:
	if (sscanf(buf, "pan_right=%s", newbuf) != EOF) {
	  set_panpoint(newbuf, RIGHT);
	}
	break;
      default:   // wrong value
	break;
      }
    }
  }
  else if (!STRCMP("pc_enable")) {
    sscanf(buf, "pc_enable=%d", &config.ccpc.pc_enable);
  }
  else if (!STRCMP("pc_cmd")) {
    if (config.mml.mml_style == CUSTOM && config.ccpc.pc_enable == 1) {
      if (sscanf(buf, "pc_cmd=%s", newbuf) != EOF) {
	config.ccpc.pc_cmd = sub_confstr(newbuf);
      }
    }
  }
  /*--- config.tempo ---*/
  else if (!STRCMP("tempo_enable")) {
    sscanf(buf, "tempo_enable=%d", &config.tempo.tempo_enable);
  }
  else if (!STRCMP("tempo_cmd")) {
    if (config.mml.mml_style == CUSTOM) {
      if (sscanf(buf, "tempo_cmd=%s", newbuf) != EOF) {
	config.tempo.tempo_cmd = sub_confstr(newbuf);
      }
    }
  }
  else if (!STRCMP("make_tempopart")) {
    if (config.tempo.tempo_enable == 1) {
      sscanf(buf, "make_tempopart=%d", &config.tempo.make_tempopart);
    }
  }
  /*--- config.track ---*/
  else if (!STRCMP("print_track")) {
    sscanf(buf, "print_track=%d", &config.track.print_track);
  }
  else if (!STRCMP("number_track")) {
    if (config.track.print_track == 2) {
      if (sscanf(buf, "number_track=%s", newbuf) != EOF) {
	set_number_track(newbuf);
      }
    }
  }
  else if (!STRCMP("print_partname")) {
    sscanf(buf, "print_partname=%d", &config.track.print_partname);
  }
  else if (!STRCMP("fmp7_autoname")) {
    if (config.mml.mml_style == FMP7 && config.track.print_partname == 1) {
      sscanf(buf, "fmp7_autoname=%d", &config.track.fmp7_autoname);
    }
  }
  else if (!STRCMP("fmp_autoname")) {
    if (config.mml.mml_style == FMP && config.track.print_partname == 1) {
      sscanf(buf, "fmp_autoname=%d", &config.track.fmp_autoname);
    }
  }
  else if (!STRCMP("pmd_autoname")) {
    if (config.mml.mml_style == PMD && config.track.print_partname == 1) {
      sscanf(buf, "pmd_autoname=%d", &config.track.pmd_autoname);
    }
  }
  else if (!STRCMP("nrtdrv_autoname")) {
    if (config.mml.mml_style == NRTDRV && config.track.print_partname == 1) {
      sscanf(buf, "nrtdrv_autoname=%d", &config.track.nrtdrv_autoname);
    }
  }
  else if (!STRCMP("name_part")) {
    if (config.track.print_partname == 2) {
      if (sscanf(buf, "name_part%s", newbuf) != EOF) {
	set_partname(newbuf);
      }
    }
  }
  else if (!STRCMP("name_tempopart")) {
    if (config.tempo.tempo_enable == 1 && config.tempo.make_tempopart == 1) {
      if (sscanf(buf, "name_tempopart=%s", newbuf) != EOF) {
	config.track.name_tempopart = sub_confstr(newbuf);
      }
    }
  }
    
  return;
}


char *sub_confstr(char *buf)
{
  int len;
  char *confstr;
  
  len = strlen(buf);

  if (buf[0] == buf[len-1]) {
    switch (buf[0]) {
    case '\"':
    case '\'':
      len -= 2;
      confstr = (char *)malloc(sizeof(char)*(len+1));
      if (confstr == NULL) {
	EPRINTM();
	free_config();
	ERREXIT();
      }
      strncpy(confstr, buf+1, len);
      confstr[len] = '\0';
      break;
    default:
      confstr = (char *)malloc(sizeof(char)*(len+1));
      if (confstr == NULL) {
	EPRINTM();
	free_config();
	ERREXIT();
      }
      strncpy(confstr, buf, len);
      confstr[len] = '\0';    
      break;
    }
  }
  else {
    confstr = (char *)malloc(sizeof(char)*(len+1));
    if (confstr == NULL) {
      EPRINTM();
      free_config();
      ERREXIT();
    }
    strncpy(confstr, buf, len);
    confstr[len] = '\0';
  }

  return confstr;
}


void set_panpoint(char *buf, enum _PANPLACE place)
{
  int value;

  value = atoi(buf);
  
  if (buf[0] == '+' || buf[0] == '-') {
    value += 64;
  }

  switch (place) {
  case LEFT:
    config.ccpc.pan_left = value;
    break;
  case RIGHT:
    config.ccpc.pan_right = value;
    break;
  }

  return;
}


void set_number_track(char *buf)
{
  int i=0, j;
  int hyphenflg=0, endflg=0;
  int trackno=0, tracknofrom=0;
  char c;


  while ( (c = *(buf + i)) != '\0' ) {

    if (0x30 <= c && c <= 0x39) {   // number
      trackno *= 10;
      switch (c) {
      case 0x30: trackno += 0; break;
      case 0x31: trackno += 1; break;
      case 0x32: trackno += 2; break;
      case 0x33: trackno += 3; break;
      case 0x34: trackno += 4; break;
      case 0x35: trackno += 5; break;
      case 0x36: trackno += 6; break;
      case 0x37: trackno += 7; break;
      case 0x38: trackno += 8; break;
      case 0x39: trackno += 9; break;
      }
      endflg = 0;
    }
    else if (c == 0x2c) {   // comma
      if (hyphenflg == 0) {
	if (trackno > 0) {
	  sort_number_track(trackno);
	}
	else {
	  EPRINTI("Track&Part", "number_track");
	  free_config();
	  ERREXIT();
	}
      }
      else {
	if (tracknofrom < trackno) {
	  for (j=tracknofrom; j<=trackno; j++) {
	    sort_number_track(j);
	  }
	  tracknofrom = 0;
	  trackno = 0;
	  hyphenflg = 0;
	}
	else {
	  EPRINTI("Track&Part", "number_track");
	  free_config();
	  ERREXIT();
	}
      }
    }
    else if (c == 0x2d) {   // hyphen
      if (trackno > 0) {
	tracknofrom = trackno;
	trackno = 0;
	hyphenflg = 1;
	endflg = 1;
      }
      else {
	EPRINTI("Track&Part", "number_track");
	free_config();
	ERREXIT();
      }
    }
    else {
      break;
    }

    i++;

  }


  if (hyphenflg == 0) {
    if (trackno > 0) {
      sort_number_track(trackno);
    }
    else {
      EPRINTI("Track&Part", "number_track");
      free_config();
      ERREXIT();
    }
  }
  else{
    if (trackno > 0) {
      for (i=tracknofrom; i<=trackno; i++) {
	sort_number_track(i);
      }
    }
    else {
      if (endflg == 1) {
	for (i=tracknofrom; i<=PART_SIZE; i++) {
	  sort_number_track(i);
	}
      }
      else {
	EPRINTI("Track&Part", "number_track");
	free_config();
	ERREXIT();
      }
    }
  }


  return;
}


void sort_number_track(int n)
{
  int i=0, tmp1, tmp2;

  while (config.track.number_track[i] < n &&
	 config.track.number_track[i] > 0) {
    i++;
    if (i == 64) {
      fprintf(stderr, "[ERROR] \"ConvFMML.ini\"の[Track&Part]部、\"number_track\"で指定した出力トラック数が%dを超えています。\n", PART_SIZE);
      free_config();
      ERREXIT();
    }
  }
  
  
  if (config.track.number_track[i] == n) {
    fprintf(stderr, " >> \"ConvFMML.ini\"の[Track&Part]部、\"number_track\"での出力トラックの指定が重複しています\n");
  }
  else {
    tmp1 = n;
    do {
      tmp2 = config.track.number_track[i];
      config.track.number_track[i] = tmp1;
      tmp1 = tmp2;
      i++;
    } while (i < 64 && tmp1 > 0);
    if (i == 64 && tmp1 > 0) {
      fprintf(stderr, "[ERROR] \"ConvFMML.ini\"の[Track&Part]部、\"number_track\"で指定した出力トラック数が64を超えています。\n");
      free_config();
      ERREXIT();
    }
  }

  return;
}


void set_partname(char *buf)
{
  int i=0, partno=0;
  char *tmp;

  while ( 0x30 <= *(buf + i) && *(buf + i) <= 0x39 ) {

    partno *= 10;
    switch (*(buf + i)) {
    case 0x30: partno += 0; break;
    case 0x31: partno += 1; break;
    case 0x32: partno += 2; break;
    case 0x33: partno += 3; break;
    case 0x34: partno += 4; break;
    case 0x35: partno += 5; break;
    case 0x36: partno += 6; break;
    case 0x37: partno += 7; break;
    case 0x38: partno += 8; break;
    case 0x39: partno += 9; break;
    }
    i++;
  }

  if (*(buf+i+1) != '\0') {
    if (partno > 0 && partno <= PART_SIZE) {
      if (config.track.name_part[partno-1] != NULL) {
	fprintf(stderr, " >> \"ConvFMML.ini\"の[Track&Part]部、\"name_part\"で、第%dパートの名前の指定が重複しています。\n", partno);
	FREE(config.track.name_part[partno-1]);
      }
      else {
	config.track.partsize = (partno > config.track.partsize)? partno : config.track.partsize;
      }
      config.track.name_part[partno-1] = sub_confstr(buf+i+1);
    }
    else {
      fprintf(stderr, "[ERROR] \"ConvFMML.ini\"の[Track&Part]部、\"name_part\"で、名前指定のできるパートの範囲を超えています。\n");
      free_config();
      ERREXIT();
    }
  }
  
  return;
}


void set_auto_partname(void)
{
  int i, cnt, tmp;
  int exitflg=0;
  
  switch (config.mml.mml_style) {

  case FMP7:
    switch (config.track.fmp7_autoname) {
    case 0:
      for (i=0; i<26; i++) {
	config.track.name_part[i] = (char *)malloc(sizeof(char)*3);
	if (config.track.name_part[i] == NULL) {
	  exitflg = 1;
	  break;
	}
	config.track.name_part[i][0] = '\'';
	config.track.name_part[i][1] = 0x41 + i;
	config.track.name_part[i][2] = '\0';
      }
      break;
    case 1:
      cnt = 0;
      for (i=0; i<PART_SIZE; i++) {
	config.track.name_part[i] = (char *)malloc(sizeof(char)*4);
	if (config.track.name_part[i] == NULL) {
	  exitflg = 1;
	  break;
	}
	config.track.name_part[i][0] = '\'';
	config.track.name_part[i][1] = 0x41 + cnt / 10;
	config.track.name_part[i][2] = 0x30 + cnt % 10;
	config.track.name_part[i][3] = '\0';
	cnt++;
      }
      break;
    case 2:
      cnt = 0;
      for (i=0; i<PART_SIZE; i++) {
	config.track.name_part[i] = (char *)malloc(sizeof(char)*4);
	if (config.track.name_part[i] == NULL) {
	  exitflg = 1;
	  break;
	}
	config.track.name_part[i][0] = '\'';
	config.track.name_part[i][1] = 0x41 + cnt % 26;
	config.track.name_part[i][2] = 0x30 + cnt / 26;
	config.track.name_part[i][3] = '\0';
	cnt++;
      }
      break;
    }
    break;

  case FMP:
    switch (config.track.fmp_autoname) {
    case 0:
      for (i=0; i<9; i++) {
	config.track.name_part[i] = (char *)malloc(sizeof(char)*3);
	if (config.track.name_part[i] == NULL) {
	  exitflg = 1;
	  break;
	}
	config.track.name_part[i][0] = '\'';
	if (i < 6) {
	  config.track.name_part[i][1] = 0x41 + i;
	}
	else {
	  config.track.name_part[i][1] = 0x58 + i - 6;
	}
	config.track.name_part[i][2] = '\0';
      }
      break;
    case 1:
      for (i=0; i<12; i++) {
	config.track.name_part[i] = (char *)malloc(sizeof(char)*3);
	if (config.track.name_part[i] == NULL) {
	  exitflg = 1;
	  break;
	}
	config.track.name_part[i][0] = '\'';
	if (i < 9) {
	  config.track.name_part[i][1] = 0x41 + i;
	}
	else {
	  config.track.name_part[i][1] = 0x58 + i - 9;
	}
	config.track.name_part[i][2] = '\0';
      }
      break;
    }
    break;
    
  case PMD:
    switch (config.track.pmd_autoname) {
    case 0:
      for (i=0; i<6; i++) {
	config.track.name_part[i] = (char *)malloc(sizeof(char)*2);
	if (config.track.name_part[i] == NULL) {
	  exitflg = 1;
	  break;
	}
	if (i < 3) {
	  config.track.name_part[i][0] = 0x41 + i;
	}
	else {
	  config.track.name_part[i][0] = 0x47 + i - 3;
	}
	config.track.name_part[i][1] = '\0';
      }
      break;
    case 1:
      for (i=0; i<9; i++) {
	config.track.name_part[i] = (char *)malloc(sizeof(char)*2);
	if (config.track.name_part[i] == NULL) {
	  exitflg = 1;
	  break;
	}
	config.track.name_part[i][0] = 0x41 + i;
	config.track.name_part[i][1] = '\0';
      }
      break;
    case 2:
      for (i=0; i<12; i++) {
	config.track.name_part[i] = (char *)malloc(sizeof(char)*2);
	if (config.track.name_part[i] == NULL) {
	  exitflg = 1;
	  break;
	}
	if (i < 9) {
	  config.track.name_part[i][0] = 0x41 + i;
	}
	else {
	  config.track.name_part[i][0] = 0x58 + i - 9;
	}
	config.track.name_part[i][1] = '\0';
      }
      break;
    case 3:
      for (i=0; i<8; i++) {
	config.track.name_part[i] = (char *)malloc(sizeof(char)*2);
	if (config.track.name_part[i] == NULL) {
	  exitflg = 1;
	  break;
	}
	config.track.name_part[i][0] = 0x41 + i;
	config.track.name_part[i][1] = '\0';
      }
      break;
    case 4:
      for (i=0; i<6; i++) {
	config.track.name_part[i] = (char *)malloc(sizeof(char)*2);
	if (config.track.name_part[i] == NULL) {
	  exitflg = 1;
	  break;
	}
	config.track.name_part[i][0] = 0x41 + i;
	config.track.name_part[i][1] = '\0';
      }
      break;
    case 5:
      for (i=0; i<9; i++) {
	config.track.name_part[i] = (char *)malloc(sizeof(char)*2);
	if (config.track.name_part[i] == NULL) {
	  exitflg = 1;
	  break;
	}
	config.track.name_part[i][0] = 0x41 + i;
	config.track.name_part[i][1] = '\0';
      }
      break;
    }
    break;

  case MXDRV:
    for (i=0; i<8; i++) {
      config.track.name_part[i] = (char *)malloc(sizeof(char)*2);
      if (config.track.name_part[i] == NULL) {
	exitflg = 1;
	break;
      }
      config.track.name_part[i][0] = 0x41 + i;
      config.track.name_part[i][1] = '\0';
    }
    break;

  case NRTDRV:
    switch (config.track.nrtdrv_autoname) {
    case 0:
      for (i=0; i<3; i++) {
	config.track.name_part[i] = (char *)malloc(sizeof(char)*2);
	if (config.track.name_part[i] == NULL) {
	  exitflg = 1;
	  break;
	}
	config.track.name_part[i][0] = 0x31 + i;
	config.track.name_part[i][1] = '\0';
      }
      break;
    case 1:
      for (i=0; i<19; i++) {
	config.track.name_part[i] = (char *)malloc(sizeof(char)*2);
	if (config.track.name_part[i] == NULL) {
	  exitflg = 1;
	  break;
	}
	if (i < 16) {
	  config.track.name_part[i][0] = 0x41 + i;
	}
	else {
	  config.track.name_part[i][0] = 0x31 + i - 16;
	}
	config.track.name_part[i][1] = '\0';
      }
      break;
    case 2:
      for (i=0; i<19; i++) {
	config.track.name_part[i] = (char *)malloc(sizeof(char)*2);
	if (config.track.name_part[i] == NULL) {
	  exitflg = 1;
	  break;
	}
	if (i < 8) {
	  config.track.name_part[i][0] = 0x41 + i;
	}
	else if (i < 11) {
	  config.track.name_part[i][0] = 0x31 + i - 8;
	}
	else {
	  config.track.name_part[i][0] = 0x49 + i - 11;
	}
	config.track.name_part[i][1] = '\0';
      }
      break;
    }
    break;
    
  }

  if (exitflg == 1) {
    EPRINTM();
    free_config();
    ERREXIT();
  }
  
  return;
}


int check_config(void)
{
  int i;
  int exitflg=0;

  /*--- config.mml ---*/
  if (!ISCONF(config.mml.mml_style, 0, 5)) {
    EPRINTI("MMLExpression", "mml_style");
    exitflg = 1;
  }
  if (config.mml.mml_style == FMP && !ISCONF(config.mml.fmp_extension, 0, 2)) {
    EPRINTI("MMLExpression", "fmp_extension");
    exitflg = 1;
  }
  if (config.mml.timebase_mml == 0) {
    EPRINTI("MMLExpression", "timebase_mml");
    exitflg = 1;
  }
  if ( (ISCONF(config.mml.mml_style, FMP7, PMD) || config.mml.mml_style == NRTDRV) && !ISCONF(config.mml.print_timebase, 0, 1)) {
    EPRINTI("MMLExpression", "print_timebase");
    exitflg = 1;
  }
  if (config.mml.mml_style == PMD && !ISCONF(config.mml.timebase_pmd, 0, 1)) {
    EPRINTI("MMLExpression", "timebase_pmd");
    exitflg = 1;
  }
  if (!ISCONF(config.mml.newblock_mes, 0, 2)) {
    EPRINTI("MMLExpression", "newblock_mes");
    exitflg = 1;
  }
  if (config.mml.newline_mes < 0) {
    EPRINTI("MMLExpression", "newline_mes");
    exitflg = 1;
  }
  if (!ISCONF(config.mml.newline_rhythm, 0, 1)) {
    EPRINTI("MMLExpression", "newline_rhythm");
    exitflg = 1;
  }
  /*--- config.note ---*/
  if (!ISCONF(config.note.octave_newline, 0, 1)) {
    EPRINTI("Note", "octave_newline");
    exitflg = 1;
  }
  if (config.mml.mml_style == CUSTOM && config.note.octave_sign == NULL) {
    EPRINTS("Note", "octave_sign");
    exitflg = 1;
  }
  if (!ISCONF(config.note.octave_direction, 0, 1)) {
    EPRINTI("Note", "octave_direction");
    exitflg = 1;
  }
  if (!ISCONF(config.note.notelength_style, 0, 1)) {
    EPRINTI("Note", "notelength_style");
    exitflg = 1;
  }
  if (config.note.range_notelength == -1) {
    EPRINTI("Note", "range_notelength");
    exitflg = 1;
  }
  else if (config.note.range_notelength > 0) {
    if (config.mml.timebase_mml % (1 << config.note.range_notelength) > 0) {
      config.note.range_notelength = 0;
    }
  }
  if (config.mml.mml_style == CUSTOM && config.note.range_notelength > 0 && config.note.clockcount_sign == NULL) {
    EPRINTS("Note", "clockcount_sign");
    exitflg = 1;
  }
  if (!ISCONF(config.note.dot_enable, 0, 1)) {
    EPRINTI("Note", "dot_enable");
    exitflg = 1;
  }
  if (config.note.dot_enable == 1 && config.note.dot_length == -1) {
    EPRINTI("Note", "dot_length");
    exitflg = 1;
  }
  if (!ISCONF(config.note.measurecut_note, 0, 2)) {
    EPRINTI("Note", "measurecut_note");
    exitflg = 1;
  }
  if (config.mml.mml_style == CUSTOM && config.note.tie_cmd == NULL) {
    EPRINTS("Note", "tie_cmd");
    exitflg = 1;
  }
  if (!ISCONF(config.note.resttie_enable, 0, 1)) {
    EPRINTI("Note", "resttie_enable");
    exitflg = 1;
  }
  if (!ISCONF(config.note.extend_note, 0, 1)) {
    EPRINTI("Note", "extend_note");
    exitflg = 1;
  }
  if (!ISCONF(config.note.tienote_block, 0, 1)) {
    EPRINTI("Note", "tienote_block");
    exitflg = 1;
  }
  /*--- config.ccpc ---*/
  if (!ISCONF(config.ccpc.overlap_ccpc, 0, 2)) {
    EPRINTI("ControlChange&ProgramChange", "overlap_ccpc");
    exitflg = 1;
  }
  if (!ISCONF(config.ccpc.invalid_ccpc, 0, 1)) {
    EPRINTI("ControlChange&ProgramChange", "invalid_ccpc");
    exitflg = 1;
  }
  if (!ISCONF(config.ccpc.replace_ccpc, 0, 2)) {
    EPRINTI("ControlChange&ProgramChange", "replace_ccpc");
    exitflg = 1;
  }
  if (!ISCONF(config.ccpc.omit_ccpc, 0, 1)) {
    EPRINTI("ControlChange&ProgramChange", "omit_ccpc");
    exitflg = 1;
  }
  if (!ISCONF(config.ccpc.volume_enable, 0, 1)) {
    EPRINTI("ControlChange&ProgramChange", "volume_enable");
    exitflg = 1;
  }
  if (config.mml.mml_style == CUSTOM && config.ccpc.volume_enable == 1 && config.ccpc.volume_cmd == NULL) {
    EPRINTS("ControlChange&ProgramChange", "volume_cmd");
    exitflg = 1;
  }
  if (config.mml.mml_style == CUSTOM && config.ccpc.volume_enable == 1 && config.ccpc.volume_range < 0) {
    EPRINTI("ControlChange&ProgramChange", "volume_range");
    exitflg = 1;
  }
  if (config.mml.mml_style == PMD && config.ccpc.volume_enable == 1 && !ISCONF(config.ccpc.volume_pmd, 0, 1)) {
    EPRINTI("ControlChange&ProgramChange", "volume_pmd");
    exitflg = 1;
  }
  if (config.mml.mml_style == MXDRV && config.ccpc.volume_enable == 1 && !ISCONF(config.ccpc.volume_mxdrv, 0, 1)) {
    EPRINTI("ControlChange&ProgramChange", "volume_mxdrv");
    exitflg = 1;
  }
  if (config.mml.mml_style == NRTDRV && config.ccpc.volume_enable == 1 && !ISCONF(config.ccpc.volume_nrtdrv, 0, 1)) {
    EPRINTI("ControlChange&ProgramChange", "volume_nrtdrv");
    exitflg = 1;
  }
  if (config.mml.mml_style == NRTDRV && config.ccpc.volume_enable == 1 && config.ccpc.volume_nrtdrv == 0 && config.ccpc.vstep_nrtdrv < 0) {
    EPRINTI("ControlChange&ProgramChange", "vstep_nrtdrv");
    exitflg = 1;
  }
  if (!ISCONF(config.ccpc.pan_enable, 0, 1)) {
    EPRINTI("ControlChange&ProgramChange", "pan_enable");
    exitflg = 1;
  }
  if (config.mml.mml_style == CUSTOM && config.ccpc.volume_enable == 1 && !ISCONF(config.ccpc.pan_cmdmode, 0, 1)) {
    EPRINTI("ControlChange&ProgramChange", "pan_cmdmode");
    exitflg = 1;
  }
  if (config.mml.mml_style == CUSTOM && config.ccpc.pan_enable == 1 && config.ccpc.pan_cmdmode == 0 && config.ccpc.pan_midicmd == NULL) {
    EPRINTS("ControlChange&ProgramChange", "pan_midicmd");
    exitflg = 1;
  }
  if (config.mml.mml_style == CUSTOM && config.ccpc.pan_enable == 1 && config.ccpc.pan_cmdmode == 1 && config.ccpc.pan_lcmd == NULL) {
    EPRINTS("ControlChange&ProgramChange", "pan_lcmd");
    exitflg = 1;
  }
  if (config.mml.mml_style == CUSTOM && config.ccpc.pan_enable == 1 && config.ccpc.pan_cmdmode == 1 && config.ccpc.pan_ccmd == NULL) {
    EPRINTS("ControlChange&ProgramChange", "pan_ccmd");
    exitflg = 1;
  }
  if (config.mml.mml_style == CUSTOM && config.ccpc.pan_enable == 1 && config.ccpc.pan_cmdmode == 1 && config.ccpc.pan_rcmd == NULL) {
    EPRINTS("ControlChange&ProgramChange", "pan_rcmd");
    exitflg = 1;
  }
  if (config.mml.mml_style == FMP7 && config.ccpc.pan_enable == 1 && !ISCONF(config.ccpc.pan_fmp7, 0, 1)) {
    EPRINTI("ControlChange&ProgramChange", "pan_fmp7");
    exitflg = 1;
  }
  if ( (config.mml.mml_style == CUSTOM || ISCONF(config.mml.mml_style, FMP, NRTDRV)) && config.ccpc.pan_enable == 1 && !ISCONF(config.ccpc.pan_left, 0, 127) ) {
    EPRINTI("ControlChange&ProgramChange", "pan_left");
    exitflg = 1;
  }
  if ( (config.mml.mml_style == CUSTOM || ISCONF(config.mml.mml_style, FMP, NRTDRV)) && config.ccpc.pan_enable == 1 && !ISCONF(config.ccpc.pan_right, 0, 127) ) {
    EPRINTI("ControlChange&ProgramChange", "pan_right");
    exitflg = 1;
  }
  if (!ISCONF(config.ccpc.pc_enable, 0, 1)) {
    EPRINTI("ControlChange&ProgramChange", "pc_enable");
    exitflg = 1;
  }
  if (config.mml.mml_style == CUSTOM && config.ccpc.pc_enable == 1 && config.ccpc.pc_cmd == NULL) {
    EPRINTS("ControlChange&ProgramChange", "pc_cmd");
    exitflg = 1;
  }
  /*--- config.tempo ---*/
  if (!ISCONF(config.tempo.tempo_enable, 0, 1)) {
    EPRINTI("Tempo", "tempo_enable");
    exitflg = 1;
  }
  if (config.mml.mml_style == CUSTOM && config.tempo.tempo_cmd == NULL) {
    EPRINTS("Tempo", "tempo_cmd");
    exitflg = 1;
  }
  if (config.tempo.tempo_enable == 1 && !ISCONF(config.tempo.make_tempopart, 0, 2)) {
    EPRINTI("Tempo", "make_tempopart");
    exitflg = 1;
  }
  /*--- config.track ---*/
  if (!ISCONF(config.track.print_track, 0, 2)) {
    EPRINTI("Track&Part", "print_track");
    exitflg = 1;
  }
  if (config.track.print_track == 2 && config.track.number_track[0] == 0) {
    EPRINTS("Track&Part", "number_track");
    exitflg = 1;
  }
  if ( !ISCONF(config.track.print_partname, 0, 2) ||
       (config.mml.mml_style == CUSTOM && config.track.print_partname == 1) ) {
    EPRINTI("Track&Part", "print_partname");
    exitflg = 1;
  }
  if (config.mml.mml_style == FMP7 && config.track.print_partname == 1 && !ISCONF(config.track.fmp7_autoname, 0, 2)) {
    EPRINTI("Track&Part", "fmp7_autoname");
    exitflg = 1;
  }
  if (config.mml.mml_style == FMP && config.track.print_partname == 1 && !ISCONF(config.track.fmp_autoname, 0, 1)) {
    EPRINTI("Track&Part", "fmp_autoname");
    exitflg = 1;
  }
  if (config.mml.mml_style == PMD && config.track.print_partname == 1 && !ISCONF(config.track.pmd_autoname, 0, 5)) {
    EPRINTI("Track&Part", "pmd_autoname");
    exitflg = 1;
  }
  if (config.mml.mml_style == NRTDRV && config.track.print_partname == 1 && !ISCONF(config.track.nrtdrv_autoname, 0, 2)) {
    EPRINTI("Track&Part", "nrtdrv_autoname");
    exitflg = 1;
  }
  if (config.track.print_partname == 2) {
    if (config.track.partsize == 0) {
      EPRINTS("Track&Part", "name_part");
      exitflg = 1;
    }
    else {
      for (i=0; i<config.track.partsize; i++) {
	if (config.track.name_part[i] == NULL) {
	  fprintf(stderr, "[ERROR] \"ConvFMML.ini\"の\"[Track&Part]\"部、\"name_part%d\"が設定されていません。\n", i+1);
	  exitflg = 1;
	}
      }
    }
  }
  if (config.tempo.tempo_enable == 1 && config.tempo.make_tempopart == 1 && config.track.name_tempopart == NULL) {
    EPRINTS("Track&Part", "name_tempopart");
    exitflg = 1;
  }

  return exitflg;
}


void set_modecmds(void)
{
  switch (config.mml.mml_style) {
  case CUSTOM:
    break;
  case FMP7:
    config.mml.mml_extension = sub_confstr("mwi");
    config.note.clockcount_sign = sub_confstr("#");
    config.note.tie_cmd = sub_confstr("&");
    config.ccpc.pan_lcmd = sub_confstr("PL");
    config.ccpc.pan_ccmd = sub_confstr("PC");
    config.ccpc.pan_rcmd = sub_confstr("PR");
    break;
  case FMP:
    switch (config.mml.fmp_extension) {
    case 0: config.mml.mml_extension = sub_confstr("mpi"); break;
    case 1: config.mml.mml_extension = sub_confstr("mvi"); break;
    case 2: config.mml.mml_extension = sub_confstr("mzi"); break;
    }
    config.note.clockcount_sign = sub_confstr("#");
    config.note.tie_cmd = sub_confstr("&");
    config.ccpc.pan_lcmd = sub_confstr("P1");
    config.ccpc.pan_ccmd = sub_confstr("P3");
    config.ccpc.pan_rcmd = sub_confstr("P2");
    break;
  case PMD:
    config.mml.mml_extension = sub_confstr("mml");
    config.note.clockcount_sign = sub_confstr("%");
    config.note.tie_cmd = sub_confstr("&");
    config.ccpc.pan_lcmd = sub_confstr("p2");
    config.ccpc.pan_ccmd = sub_confstr("p3");
    config.ccpc.pan_rcmd = sub_confstr("p1");
    break;
  case MXDRV:
    config.mml.mml_extension = sub_confstr("mml");
    config.note.clockcount_sign = sub_confstr("%");
    if (config.note.extend_note == 0) {
      config.note.tie_cmd = sub_confstr("&");
    }
    else {
      config.note.tie_cmd = sub_confstr("^");
    }
    config.ccpc.pan_lcmd = sub_confstr("p1");
    config.ccpc.pan_ccmd = sub_confstr("p3");
    config.ccpc.pan_rcmd = sub_confstr("p2");
    break;
  case NRTDRV:
    config.mml.mml_extension = sub_confstr("mml");
    config.note.clockcount_sign = sub_confstr("#");
    if (config.note.extend_note == 0) {
      config.note.tie_cmd = sub_confstr("&");
    }
    else {
      config.note.tie_cmd = sub_confstr("^");
    }
    config.ccpc.pan_lcmd = sub_confstr("P1");
    config.ccpc.pan_ccmd = sub_confstr("P3");
    config.ccpc.pan_rcmd = sub_confstr("P2");
    break;
  }

  return;
}


void set_track_and_partsize(void)
{
  switch (config.mml.mml_style) {
  case CUSTOM:
    if (config.track.print_partname == 0) {
      config.track.partsize = PART_SIZE;
    }
    break;
  case FMP7:
    if (config.track.print_partname == 1) {
      switch (config.track.fmp7_autoname) {
      case 0:
	config.track.partsize = 26;
	break;
      case 1:
      case 2:
	config.track.partsize = 64;
	break;
      }
    }
    else if (config.track.print_partname == 0) {
      config.track.partsize = PART_SIZE;
    }
    break;
  case FMP:
    if (config.track.print_partname == 1) {
      switch (config.track.fmp_autoname) {
      case 0:
	config.track.partsize = 9;
	break;
      case 1:
	config.track.partsize = 12;
	break;
      }
    }
    else if (config.track.print_partname == 0) {
      config.track.partsize = PART_SIZE;
    }
    break;
  case PMD:
    if (config.track.print_partname == 1) {
      switch (config.track.pmd_autoname) {
      case 0:
      case 4:
	config.track.partsize = 6;
	break;
      case 1:
      case 5:
	config.track.partsize = 9;
	break;
      case 2:
	config.track.partsize = 12;
	break;
      case 3:
	config.track.partsize = 8;
	break;
      }
    }
    else if (config.track.print_partname == 0) {
      config.track.partsize = PART_SIZE;
    }
    break;
  case MXDRV:
    if (config.track.print_partname == 1) {
      config.track.partsize = 8;
    }
    else if (config.track.print_partname == 0) {
      config.track.partsize = PART_SIZE;
    }
    break;
  case NRTDRV:
    if (config.track.print_partname == 1) {
      switch (config.track.fmp_autoname) {
      case 0:
	config.track.partsize = 3;
	break;
      case 1:
      case 2:
	config.track.partsize = 19;
	break;
      }
    }
    else if (config.track.print_partname == 0) {
      config.track.partsize = PART_SIZE;
    }
    break;
  }
  
  return;
}


void free_config(void)
{
  int i;

  FREE(config.mml.mml_extension);
  FREE(config.note.octave_sign);
  FREE(config.note.clockcount_sign);
  FREE(config.note.tie_cmd);
  FREE(config.ccpc.volume_cmd);
  FREE(config.ccpc.pan_midicmd);
  FREE(config.ccpc.pan_lcmd);
  FREE(config.ccpc.pan_ccmd);
  FREE(config.ccpc.pan_rcmd);
  FREE(config.ccpc.pc_cmd);
  FREE(config.tempo.tempo_cmd);
  for (i=0; i<PART_SIZE; i++) {
    FREE(config.track.name_part[i]);
  }
  FREE(config.track.name_tempopart);

  return;
}
