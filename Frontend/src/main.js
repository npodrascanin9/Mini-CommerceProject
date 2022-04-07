import Vue from 'vue'
import App from './App.vue'
import router from './router'
import PrimeVue from 'primevue/config'
import ConfirmationService from 'primevue/confirmationservice';
import ToastService from 'primevue/toastservice';

import 'primevue/resources/themes/saga-blue/theme.css'       //theme
import 'primevue/resources/primevue.min.css'                 //core css
import 'primeicons/primeicons.css'                           //icons

import 'primeflex/primeflex.css';

import VueRouter from 'vue-router'

import axios from 'axios'


axios.defaults.baseURL = 'http://localhost:9103/api'
axios.defaults.headers.get['Accepts'] = 'application/json'

Vue.config.productionTip = false


Vue.use(ToastService);
Vue.use(VueRouter);
Vue.use(PrimeVue);
Vue.use(ConfirmationService);

new Vue({
  render: h => h(App),
  router
}).$mount('#app')
