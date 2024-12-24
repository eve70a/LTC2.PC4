<template>
  
    <div ref="modalElement" tabindex="-1" aria-hidden="true"  class="fixed top-0 left-0 right-0 z-50 hidden w-full p-4 overflow-x-hidden overflow-y-auto md:inset-0 h-modal md:h-full">
      <!-- div class="relative w-full h-full max-w-2xl md:h-auto" -->
      <div class="relative w-full h-auto max-w-2xl">
          <!-- Modal content -->
          <div class="relative bg-white rounded-lg shadow dark:bg-gray-700">
              <!-- Modal header -->
              <div class="flex items-start justify-between p-4 border-b rounded-t dark:border-gray-600">
                  <h3 class="text-xl font-semibold text-gray-900 dark:text-white">
                      {{ header }}
                  </h3>
                  <button type="button" @click="hideModal()" class="text-gray-400 bg-transparent hover:bg-gray-200 hover:text-gray-900 rounded-lg text-sm p-1.5 ml-auto inline-flex items-center dark:hover:bg-gray-600 dark:hover:text-white" >
                      <svg aria-hidden="true" class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg"><path fill-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clip-rule="evenodd"></path></svg>
                      <span class="sr-only">Close modal</span>
                  </button>
              </div>
              <!-- Modal body -->
  
              <div class="pb-0 pt-2 px-2 bg-white dark:bg-gray-900">
                  <div class="mt-1 flex items-center space-x-2">                                                
                    <input type="file" accept=".gpx" class="hidden" id="fileInput" @change="onSelectFile">
                    <button onclick="document.getElementById('fileInput').click();" class="text-white bg-blue-700 hover:bg-blue-800 focus:ring-4 focus:outline-none focus:ring-blue-300 rounded-lg border border-gray-200 text-sm font-medium px-5 py-2.5 focus:z-10 dark:bg-gray-700 dark:text-gray-300 dark:border-gray-500 dark:hover:text-white dark:hover:bg-gray-600 dark:focus:ring-gray-600" type="button">{{buttonTextSelectFile}}</button>
                    <input type="text" disabled v-model="fileName" ref="inputElement" class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-80 p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500" :placeholder="texthint">                            
                    <button v-if="isButtonDisabled" disabled class="text-white bg-blue-300 rounded outline-none font-medium rounded-lg text-sm px-5 py-2.5" type="button">{{buttonTextCheckGpx}}</button>
                    <button v-else @click="onSelectGpx" ref="selectFileButtons" class="block text-white bg-blue-700 hover:bg-blue-800 focus:ring-4 focus:outline-none focus:ring-blue-300 font-medium rounded-lg text-sm px-5 py-2.5 text-center dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800" type="button">{{buttonTextCheckGpx}}</button>
                  </div>
                  <div class="p-2 space-y-2 overflow-y-clip overflow-x-clip mb-4" style="height: 30px;">
                        <p v-if="isEmpty" class="pl-2 hidden md:block">{{ feedBackNoRoutes }}</p>
                        <p v-if="isWorking" class="pl-2 hidden md:block">{{ feedBackWorking }}</p>
                  </div>
              </div>
  
              <div class="relative overflow-x-auto">
                  <div ref="tableContainer" class="p-2 space-y-2 overflow-y-scroll overflow-x-clip mb-4" style="height: 300px;">
                  </div>
              </div>
              <!-- Modal footer -->
              <div class="flex items-center p-6 space-x-2 border-t border-gray-200 rounded-b dark:border-gray-600">
              </div>
          </div>
      </div>
    </div>  
  
  </template>
  
  <script lang="ts">
  import { defineComponent, ref, onMounted, inject } from 'vue';
  import { Modal } from 'flowbite';
  
  import { AppTypes } from '../types/AppTypes';
  import { Routes } from '../models/Routes';
  import { emptyString } from '@/models/Constants';
  
  interface HTMLInputEvent extends Event {
    target: HTMLInputElement & EventTarget
  }

  export default defineComponent({
  
      emits: [ 'error', 'routeRequested' ],
  
      setup (_, { emit }) {
  
          const _translationService = inject(AppTypes.ITranslationServiceKey);
          const _settingsService = inject(AppTypes.ISettingsServiceKey);
          const _routeCheckerService = inject(AppTypes.IRouteCheckerService);
         
          const modalElement = ref<HTMLElement>();
          const tableContainer = ref<HTMLDivElement>();
          const inputElement = ref<HTMLInputElement>();
          const selectFileButtons = ref<HTMLButtonElement>();
          const fileName = ref(emptyString);
          const feedBackNoRoutes =  _translationService?.getText("selectroutemodal.text.noroutes");
          const feedBackWorking =  _translationService?.getText("selectroutemodal.text.working");

          const header = _translationService?.getText("selectroutemodal.header");
          const texthint = _translationService?.getText("selectroutemodal.text.hint");
          const buttonTextSelectFile = _translationService?.getText("selectroutemodal.button.selectfile");
          const buttonTextCheckGpx = _translationService?.getText("selectroutemodal.button.checkgpx");
  
          const isButtonDisabled = ref(true);
          const isEmpty = ref(false);
          const isWorking = ref(false);

          let modal: Modal;
          let selectedFiles: FileList | undefined;
         
          onMounted(() => {
              modal = new Modal(modalElement?.value);
          }) 
  
          const settings = _settingsService?.getSettingsSync();
          const checkGpxUrl = settings?.routeServiceBaseUrl + "/api/Route/checkgpx";

          const showModal = () => {
            isButtonDisabled.value = fileName.value == "";
            isEmpty.value = false;
            isWorking.value = false;
            
            modal.show();
          }
  
          const hideModal = () => {
              modal.hide();
          }

          const onSelectFile = (event: Event) => {
            let files = (event as HTMLInputEvent).target.files;

            if (!files?.length) {
                return;
            }

            console.log("selectFile: " + files[0].name)

            if (files && files.length > 0 && files[0].name) {
                fileName.value = files[0].name;

                selectedFiles = files;

                isButtonDisabled.value = false;
                isEmpty.value = false;
            }
          }

          const getPlaces = (routes: Routes) => {
            let places : string[] = [];

            if (routes && routes.routeCollection){
                routes.routeCollection.forEach(r => {
                    if (r.places){
                        places = [...places, ...r.places]
                    }
                })
            }

            return places;
          }

          const hasPlaces = (routes: Routes) => {
            return getPlaces(routes).length > 0;
          }

          const onSelectGpx = async () => {
            isButtonDisabled.value = true;
            isWorking.value = true;

            let files = selectedFiles;

            if (!files?.length) {
                return;
            }

            try {
                var route = await _routeCheckerService?.checkGpx(files[0]);

                if (route && hasPlaces(route)) {
                    emit('routeRequested', route);

                    hideModal();
                }
                else {
                    isEmpty.value = true;
                }                        
            }
            catch (error) {
                console.log("error when selecting track: " + error);
                            
                hideModal();

                emit('error', error);                     
            }

            isButtonDisabled.value = false;
            isWorking.value = false;
          }
    
          return { showModal, hideModal, modalElement, header, tableContainer, texthint, onSelectFile, inputElement, fileName, buttonTextSelectFile, checkGpxUrl, selectFileButtons, isButtonDisabled, onSelectGpx, buttonTextCheckGpx, feedBackNoRoutes, feedBackWorking, isEmpty, isWorking }
      }
  })
  
  </script>
  
  <style lang="scss" scoped>
  
  </style>
