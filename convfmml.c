#include <string.h>
#include "convfmml.h"
#include "config.h"
#include "input.h"
#include "analyze.h"
#include "modify.h"
#include "output.h"


struct _CONFIG config;

unsigned short timebase;
double tbratio;
pos_t totalpos={
  .measure=1,
  .miditick=0,
  .mmltick=0
};

marker_t *marker_head=NULL;
tempo_t *tempo_head=NULL;
rhythm_t *rhythm_head=NULL;
keysig_t *keysig_head=NULL;

elist_t *t_part=NULL;


int main(int argc, char *argv[])
{
  int i;
  
  unsigned short tracks_num;
  unsigned int t_partnum=0;   // initialize

  struct _CHUNK *chunkdata=NULL;
  track_t *track=NULL;
  elist_t **seq=NULL;
  
  char *outfilename=NULL;


  puts("===============================================================================");
  puts("");
  puts("\t\t---  MIDI->MML Converter  ConvFMML  ---");
  puts("\t\t\t\t\tVer 0.34    Last Update: 2017/04/15");
  puts("");
  puts("===============================================================================");
  puts("");
  
  
  /*--- load ConvFMML.ini ---*/
  load_config(argv[0]);
  puts(" > \"ConvFMML.ini\"の読み込み完了");

  
  /*--- check usage and load MIDI ---*/
  if (argc == 2 || argc == 3) {
    if (!ISEXT("mid") && !ISEXT("MID") && !ISEXT("midi") &&
	!ISEXT("MIDI") && !ISEXT("smf") && !ISEXT("SMF")) {
      fprintf(stderr, "[ERROR] \"%s\"はMIDIデータではありません。\n", argv[1]);
      free_config();
      ERREXIT();
    }
    else {
      chunkdata = load_midi(argv[1], &tracks_num);
    }
  }
  else {
    fprintf(stderr, "[ERROR] 正しい引数を入力してください。\n");
    free_config();
    ERREXIT();
  }

  track = func_analyze(chunkdata, tracks_num);
  free_chunkdata(chunkdata, tracks_num);
  puts(" > MIDIデータ読込完了");


  /*--- modify last rhythm end ---*/
  modlastrhythm();

  
  /*--- delete invalid track and note ---*/
  track = func_modify_track(track);
  puts(" > 不要なMIDIトラックの削除完了");


  /*--- modify overlapping ccpc and cut rest by ccpc ---*/
  func_modify_ccpc(track);
  puts(" > コントロールチェンジ、プログラムチェンジの整形完了");

  
  /*--- make event sequence ---*/
  seq = create_eventlists(track);
  puts(" > MIDIイベントのソ\ート完了");


  /*--- analyze tempo track and modify note and ccpc ---*/
  if (config.tempo.tempo_enable == 1) {
    t_partnum = isinsert_tempo(seq);
    puts(" > テンポコマンド挿入の確認完了");
  }
  else {
    t_partnum = 0;
  }

  
  /*--- modify track to print tempo event ---*/
  if (config.tempo.tempo_enable == 1) {
    seq = modify_tempopart(seq, &t_partnum);
    puts(" > テンポコマンド挿入完了");
  }


  /*--- cut note by measure ---*/
  seq = cut_note_by_measure(seq, t_partnum);
  puts(" > 休符の整形完了");


  /* /\*--- show event sequence ---*\/ */
  /* show_events(seq); */
  

  /*--- format and print out data ---*/
  if (argc == 2) {
    outfilename = make_outfilename(argv[1]);
  }
  else {
    outfilename = argv[2];
  }
  if (isoutput(outfilename)){
    print_events(outfilename, seq, t_partnum);
    puts(" > MML出力完了");
    puts("\n > 変換完了");
  }
  else {
    fprintf(stderr, " >> MML出力がキャンセルされました。\n");
  }
  
  
  /*=== free data ===*/
  FREE(outfilename);
  free_event_list(seq);
  free_events(t_part);
  free_track_list(track);
  free_metalists();
  free_config();

  
  puts("");
  WAIT();

  
  return 0;
}
