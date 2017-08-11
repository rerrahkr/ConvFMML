#include <string.h>
#include "convfmml.h"
#include "input.h"


char *getfileextend(char *filename)
{
  char *dot;

  dot = strrchr(filename, '.');
  if (dot == NULL) {
    return NULL;
  }

  return dot + 1;
}


struct _CHUNK *load_midi(char *infilename, unsigned short *num)
{
  int i;
  int lendian=islendian();   // little-endian flag
  char chunktype[4];
  unsigned int chunksize;
  struct _CHUNK *chunkdata;
  FILE *fp;


  /*--- MIDI file open ---*/
  fp = fopen(infilename, "rb");
  if (fp == NULL) {
    fprintf(stderr, "[ERROR] \"%s\"をオープンできません。\n", infilename);
    free_config();
    ERREXIT();
  }
  
  
  /*--- read header chunk --*/
  fread(chunktype, sizeof(char), 4, fp);
  fread(&chunksize, sizeof(unsigned int), 1, fp);
  fseek(fp, 2, SEEK_CUR);
  fread(num, sizeof(unsigned short), 1, fp);
  fread(&timebase, sizeof(short), 1, fp);

  if (lendian) {
    convert_lendian(&chunksize, sizeof(chunksize));
    convert_lendian(num, sizeof(*num));
    convert_lendian(&timebase, sizeof(timebase));
  }
  timebase *= 4;   // modify timebase to the length of whole note
  tbratio = (double)config.mml.timebase_mml / (double)timebase;
  
  if (memcmp("MThd", chunktype, 4)) {
    fprintf(stderr, "[ERROR] データが壊れています。\n");
    fclose(fp);
    free_config();
    ERREXIT();
  }

  
  /*--- make array for tracks and chunk data ---*/
  chunkdata = (struct _CHUNK *)malloc(sizeof(struct _CHUNK)*(*num));
  if (chunkdata == NULL) {
    EPRINTM();
    fclose(fp);
    FREE(chunkdata);
    free_config();
    ERREXIT();
  }
  

  /*--- make track lists ---*/
  for (i=0; i<*num; i++) {
    
    chunkdata[i].event = NULL;   // initialize

    fread(chunktype, sizeof(char), 4, fp);
    fread(&chunkdata[i].size, sizeof(unsigned int), 1, fp);
    if (lendian) {
      convert_lendian(&chunkdata[i].size, sizeof(chunksize));
    }
    if (memcmp("MTrk", chunktype, 4)) {
      fprintf(stderr, "[ERROR] データが壊れています。\n");
      fclose(fp);
      free_chunkdata(chunkdata, *num);
      free_config();
      ERREXIT();
    }

    /*--- insert binary data in array ---*/
    chunkdata[i].event = (byte_t *)malloc(sizeof(byte_t)*chunkdata[i].size);
    if (chunkdata[i].event == NULL) {
      EPRINTM();
      fclose(fp);
      free_chunkdata(chunkdata, *num);
      free_config();
      ERREXIT();
    }
    fread(chunkdata[i].event, sizeof(byte_t), chunkdata[i].size, fp);
    
  }


  fclose(fp);


  return chunkdata;
}


int islendian(void)
{
  int a=1;

  if (*(char *)&a) {
    return 1;
  }
  else {
    return 0;
  }

  /* return; */
}


void convert_lendian(void *s, size_t size)
{
  int i;
  char *tmp;

  tmp = (char *)malloc(sizeof(char)*size);
  if (tmp == NULL) {
    EPRINTM();
    free_config();
    ERREXIT();
  }

  memcpy(tmp, (char *)s, size);

  for (i=0; i<size; i++) {
    ((char *)s)[i] = tmp[(size-1)-i];
  }

  FREE(tmp);

  return;
}


void free_chunkdata(struct _CHUNK *data, unsigned short num)
{
  int i;

  for (i=0; i<num; i++) {
    FREE(data[i].event);
  }
  FREE(data);

  return;
}
