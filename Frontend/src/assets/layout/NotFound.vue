<template>
  <section>
    <Card style="width: 25rem; margin-bottom: 2em">
      <template #title>Page Not Found</template>
      <template #content>
        <p>
            This page could not be found, check out all our products
            <router-link to="/products">products</router-link>
        </p>
      </template>
    </Card>
  </section>
</template>

<script>
import Card from 'primevue/card';

export default {
    name: 'NotFound',
    components: {
        Card
    }
}
</script>